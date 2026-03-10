using System.Globalization;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using OperationIntelligence.Core;
using OperationIntelligence.DB;

internal static class WorkbookSeedRunner
{
    private const string SeedUser = "seed-script";

    private static readonly string DataDirectory = Path.Combine("scripts", "data");
    private static readonly string AuthWorkbookPath = Path.Combine(DataDirectory, "Auth_seed_Dataset.xlsx");
    private static readonly string InventoryWorkbookPath = Path.Combine(DataDirectory, "inventory_seed_data_relational.xlsx");
    private static readonly string OrdersWorkbookPath = Path.Combine(DataDirectory, "order_seed_data_250_relational.xlsx");
    private static readonly string ProductionWorkbookPath = Path.Combine(DataDirectory, "production_seed_data_relational.xlsx");
    private static readonly string SchedulingWorkbookPath = Path.Combine(DataDirectory, "scheduling_seed_data_with_auth_relational.xlsx");
    private static readonly string ShipmentWorkbookPath = Path.Combine(DataDirectory, "shipment_seed_data_relational.xlsx");

    private sealed record NormalizedProductMap(Guid ProductId, Guid? UnitOfMeasureId);

    public static async Task RunAsync(string[] args)
    {
        var summaryOnly = HasFlag(args, "--summary-only");
        var resetDatabase = HasFlag(args, "--reset");

        var connectionString = GetArg(args, "--connection") ?? LoadConnectionStringFromApiAppSettings();
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string was not provided and could not be loaded from OperationIntelligence.Api/appsettings.json");
        }

        var dbOptions = new DbContextOptionsBuilder<OperationIntelligenceDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        await using var context = new OperationIntelligenceDbContext(dbOptions);

        if (resetDatabase)
        {
            Console.WriteLine("Reset flag detected. Dropping and recreating database...");
            await context.Database.EnsureDeletedAsync();
        }

        await context.Database.MigrateAsync();

        if (!await context.Database.CanConnectAsync())
        {
            throw new InvalidOperationException("Cannot connect to database.");
        }

        if (summaryOnly)
        {
            await PrintSummaryAsync(context);
            return;
        }

        Console.WriteLine("Seeding from workbook files in scripts/data ...");

        await SeedAuthAsync(context);
        await SeedInventoryAsync(context);
        await SeedOrdersAsync(context);
        await SeedProductionAsync(context);
        await SeedSchedulingAsync(context);
        await SeedShipmentAsync(context);

        Console.WriteLine("Workbook seeding complete.");
        await PrintSummaryAsync(context);
    }

    private static async Task SeedAuthAsync(OperationIntelligenceDbContext context)
    {
        if (await context.Users.AnyAsync())
        {
            Console.WriteLine("Auth data already exists. Skipping auth workbook import.");
            return;
        }

        EnsureWorkbookExists(AuthWorkbookPath);
        var workbook = ReadWorkbookSheets(AuthWorkbookPath);
        var passwordService = new PasswordService();

        var roleIdMap = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);
        var roles = Sheet(workbook, "Roles")
            .Select(row =>
            {
                var workbookRoleId = GetValue(row, "Id");
                var roleId = Guid.TryParse(workbookRoleId, out var parsedRoleId) ? parsedRoleId : Guid.NewGuid();
                roleIdMap[workbookRoleId] = roleId;

                return new Role
                {
                    Id = roleId,
                    Name = GetValue(row, "Name"),
                    NormalizedName = NormalizeUpper(GetValue(row, "Name")),
                    IsSystemRole = true,
                    CreatedAtUtc = DateValue(row, "CreatedAtUtc", DateTime.UtcNow),
                    CreatedBy = ValueOrDefault(row, "CreatedBy", SeedUser)
                };
            })
            .ToList();

        await context.Roles.AddRangeAsync(roles);

        var users = new List<PlatformUser>();
        var profiles = new List<PlatformUserProfile>();
        var passwordHistories = new List<PasswordHistory>();
        var usedUserNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var row in Sheet(workbook, "Users"))
        {
            var userId = ParseGuid(row, "Id");
            var email = GetValue(row, "Email");
            var firstName = GetValue(row, "FirstName");
            var lastName = GetValue(row, "LastName");
            var createdAt = DateValue(row, "CreatedAtUtc", DateTime.UtcNow);
            var userName = BuildUniqueUserName(email, firstName, lastName, usedUserNames);
            var passwordHash = passwordService.HashPassword(ValueOrDefault(row, "Password", "ChangeMe123!"));

            users.Add(new PlatformUser
            {
                Id = userId,
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Avatar = ValueOrDefault(row, "Avatar", string.Empty),
                NormalizedEmail = NormalizeUpper(email),
                UserName = userName,
                NormalizedUserName = NormalizeUpper(userName),
                PasswordHash = passwordHash,
                EmailConfirmed = true,
                PhoneNumberConfirmed = !string.IsNullOrWhiteSpace(ValueOrNull(row, "PhoneNumber")),
                TwoFactorEnabled = false,
                IsActive = true,
                IsLocked = false,
                LastLoginAtUtc = createdAt,
                PasswordChangedAtUtc = createdAt,
                AuthProvider = "Local",
                CreatedAtUtc = createdAt,
                CreatedBy = ValueOrDefault(row, "CreatedBy", SeedUser)
            });

            profiles.Add(new PlatformUserProfile
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                FirstName = firstName,
                LastName = lastName,
                DisplayName = $"{firstName} {lastName}".Trim(),
                Birthdate = DateValueOrNull(row, "Birthdate"),
                Gender = ValueOrNull(row, "Gender"),
                PhoneNumber = ValueOrNull(row, "PhoneNumber"),
                AddressLine1 = ValueOrNull(row, "Address"),
                City = ValueOrNull(row, "City"),
                StateOrProvince = ValueOrNull(row, "State"),
                Country = ValueOrNull(row, "Country"),
                PostalCode = ValueOrNull(row, "PostalCode"),
                CreatedAtUtc = createdAt,
                CreatedBy = ValueOrDefault(row, "CreatedBy", SeedUser)
            });

            passwordHistories.Add(new PasswordHistory
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                PasswordHash = passwordHash,
                CreatedAtUtc = createdAt,
                CreatedBy = SeedUser
            });
        }

        await context.Users.AddRangeAsync(users);
        await context.UserProfiles.AddRangeAsync(profiles);
        await context.PasswordHistories.AddRangeAsync(passwordHistories);

        var userRoles = Sheet(workbook, "UserRoles")
            .Select(row => new UserRole
            {
                UserId = ParseGuid(row, "UserId"),
                RoleId = ResolveMappedGuid(GetValue(row, "RoleId"), roleIdMap)
            })
            .ToList();
        await context.UserRoles.AddRangeAsync(userRoles);

        var refreshTokens = Sheet(workbook, "RefreshTokens")
            .Select(row =>
            {
                var rawToken = ValueOrDefault(row, "Token", Guid.NewGuid().ToString("N"));
                return new RefreshToken
                {
                    Id = ParseGuid(row, "Id"),
                    UserId = ParseGuid(row, "UserId"),
                    TokenHash = HashToken(rawToken),
                    ExpiresAtUtc = DateValue(row, "ExpiresAt", DateTime.UtcNow.AddDays(7)),
                    RevokedAtUtc = BoolValue(row, "Revoked", false) ? DateTime.UtcNow : null,
                    CreatedAtUtc = DateValue(row, "CreatedAtUtc", DateTime.UtcNow),
                    CreatedBy = ValueOrDefault(row, "CreatedBy", SeedUser)
                };
            })
            .ToList();
        await context.RefreshTokens.AddRangeAsync(refreshTokens);

        var userEmailById = users.ToDictionary(user => user.Id, user => user.Email);
        var loginAttempts = Sheet(workbook, "LoginAttempts")
            .Select(row =>
            {
                var userId = GuidValueOrNull(row, "UserId");
                return new LoginAttempt
                {
                    Id = ParseGuid(row, "Id"),
                    UserId = userId,
                    Email = userId.HasValue && userEmailById.TryGetValue(userId.Value, out var email) ? email : null,
                    IpAddress = ValueOrNull(row, "IpAddress"),
                    IsSuccessful = BoolValue(row, "Success", false),
                    FailureReason = BoolValue(row, "Success", false) ? null : "Imported failed login",
                    CreatedAtUtc = DateValue(row, "CreatedAtUtc", DateTime.UtcNow),
                    CreatedBy = ValueOrDefault(row, "CreatedBy", SeedUser)
                };
            })
            .ToList();
        await context.LoginAttempts.AddRangeAsync(loginAttempts);

        await context.SaveChangesAsync();
        Console.WriteLine($"Imported auth workbook: Roles={roles.Count}, Users={users.Count}, RefreshTokens={refreshTokens.Count}");
    }

    private static async Task SeedInventoryAsync(OperationIntelligenceDbContext context)
    {
        if (await context.Products.AnyAsync())
        {
            Console.WriteLine("Inventory data already exists. Skipping inventory workbook import.");
            return;
        }

        EnsureWorkbookExists(InventoryWorkbookPath);
        var workbook = ReadWorkbookSheets(InventoryWorkbookPath);

        var categories = Sheet(workbook, "Categories")
            .Select(row => new Category
            {
                Id = ParseGuid(row, "Id"),
                Name = GetValue(row, "Name"),
                Description = ValueOrNull(row, "Description"),
                ParentCategoryId = GuidValueOrNull(row, "ParentCategoryId"),
                CreatedAtUtc = DateValue(row, "CreatedAtUtc", DateTime.UtcNow),
                UpdatedAtUtc = DateValueOrNull(row, "UpdatedAtUtc"),
                IsDeleted = BoolValue(row, "IsDeleted", false),
                DeletedAtUtc = DateValueOrNull(row, "DeletedAtUtc"),
                CreatedBy = ValueOrDefault(row, "CreatedBy", SeedUser)
            })
            .ToList();
        await context.Categories.AddRangeAsync(categories);

        var brands = Sheet(workbook, "Brands")
            .Select(row => new Brand
            {
                Id = ParseGuid(row, "Id"),
                Name = GetValue(row, "Name"),
                Description = ValueOrNull(row, "Description"),
                CreatedAtUtc = DateValue(row, "CreatedAtUtc", DateTime.UtcNow),
                UpdatedAtUtc = DateValueOrNull(row, "UpdatedAtUtc"),
                IsDeleted = BoolValue(row, "IsDeleted", false),
                DeletedAtUtc = DateValueOrNull(row, "DeletedAtUtc"),
                CreatedBy = ValueOrDefault(row, "CreatedBy", SeedUser)
            })
            .ToList();
        await context.Brands.AddRangeAsync(brands);

        var uoms = Sheet(workbook, "UnitOfMeasures")
            .Select(row => new UnitOfMeasure
            {
                Id = ParseGuid(row, "Id"),
                Name = GetValue(row, "Name"),
                Symbol = GetValue(row, "Symbol"),
                CreatedAtUtc = DateValue(row, "CreatedAtUtc", DateTime.UtcNow),
                UpdatedAtUtc = DateValueOrNull(row, "UpdatedAtUtc"),
                IsDeleted = BoolValue(row, "IsDeleted", false),
                DeletedAtUtc = DateValueOrNull(row, "DeletedAtUtc"),
                CreatedBy = ValueOrDefault(row, "CreatedBy", SeedUser)
            })
            .ToList();
        await context.UnitsOfMeasure.AddRangeAsync(uoms);

        var warehouses = Sheet(workbook, "Warehouses")
            .Select(row => new Warehouse
            {
                Id = ParseGuid(row, "Id"),
                Name = GetValue(row, "Name"),
                Code = GetValue(row, "Code"),
                Description = ValueOrNull(row, "Description"),
                AddressLine1 = ValueOrNull(row, "AddressLine1"),
                AddressLine2 = ValueOrNull(row, "AddressLine2"),
                City = ValueOrNull(row, "City"),
                StateOrProvince = ValueOrNull(row, "StateOrProvince"),
                PostalCode = ValueOrNull(row, "PostalCode"),
                Country = ValueOrNull(row, "Country"),
                IsActive = BoolValue(row, "IsActive", true),
                CreatedAtUtc = DateValue(row, "CreatedAtUtc", DateTime.UtcNow),
                UpdatedAtUtc = DateValueOrNull(row, "UpdatedAtUtc"),
                IsDeleted = BoolValue(row, "IsDeleted", false),
                DeletedAtUtc = DateValueOrNull(row, "DeletedAtUtc"),
                CreatedBy = ValueOrDefault(row, "CreatedBy", SeedUser)
            })
            .ToList();
        await context.Warehouses.AddRangeAsync(warehouses);

        var suppliers = Sheet(workbook, "Suppliers")
            .Select(row => new Supplier
            {
                Id = ParseGuid(row, "Id"),
                Name = GetValue(row, "Name"),
                ContactPerson = ValueOrNull(row, "ContactPerson"),
                Email = ValueOrNull(row, "Email"),
                PhoneNumber = ValueOrNull(row, "PhoneNumber"),
                AddressLine1 = ValueOrNull(row, "AddressLine1"),
                AddressLine2 = ValueOrNull(row, "AddressLine2"),
                City = ValueOrNull(row, "City"),
                StateOrProvince = ValueOrNull(row, "StateOrProvince"),
                PostalCode = ValueOrNull(row, "PostalCode"),
                Country = ValueOrNull(row, "Country"),
                IsActive = BoolValue(row, "IsActive", true),
                Notes = ValueOrNull(row, "Notes"),
                CreatedAtUtc = DateValue(row, "CreatedAtUtc", DateTime.UtcNow),
                UpdatedAtUtc = DateValueOrNull(row, "UpdatedAtUtc"),
                IsDeleted = BoolValue(row, "IsDeleted", false),
                DeletedAtUtc = DateValueOrNull(row, "DeletedAtUtc"),
                CreatedBy = ValueOrDefault(row, "CreatedBy", SeedUser)
            })
            .ToList();
        await context.Suppliers.AddRangeAsync(suppliers);

        var products = Sheet(workbook, "Products")
            .Select(row => new Product
            {
                Id = ParseGuid(row, "Id"),
                Name = GetValue(row, "Name"),
                Description = ValueOrNull(row, "Description"),
                SKU = GetValue(row, "SKU"),
                Barcode = ValueOrNull(row, "Barcode"),
                CategoryId = ParseGuid(row, "CategoryId"),
                BrandId = GuidValueOrNull(row, "BrandId"),
                UnitOfMeasureId = ParseGuid(row, "UnitOfMeasureId"),
                CostPrice = DecimalValue(row, "CostPrice"),
                SellingPrice = DecimalValue(row, "SellingPrice"),
                TaxRate = DecimalValue(row, "TaxRate"),
                ReorderLevel = DecimalValue(row, "ReorderLevel"),
                ReorderQuantity = DecimalValue(row, "ReorderQuantity"),
                TrackInventory = BoolValue(row, "TrackInventory", true),
                AllowBackOrder = BoolValue(row, "AllowBackOrder", false),
                IsSerialized = BoolValue(row, "IsSerialized", false),
                IsBatchTracked = BoolValue(row, "IsBatchTracked", false),
                IsPerishable = BoolValue(row, "IsPerishable", false),
                Weight = DecimalValue(row, "Weight"),
                Length = DecimalValue(row, "Length"),
                Width = DecimalValue(row, "Width"),
                Height = DecimalValue(row, "Height"),
                Status = EnumValue(row, "Status", ProductStatus.Active),
                ThumbnailImageUrl = ValueOrNull(row, "ThumbnailImageUrl"),
                CreatedAtUtc = DateValue(row, "CreatedAtUtc", DateTime.UtcNow),
                UpdatedAtUtc = DateValueOrNull(row, "UpdatedAtUtc"),
                IsDeleted = BoolValue(row, "IsDeleted", false),
                DeletedAtUtc = DateValueOrNull(row, "DeletedAtUtc"),
                CreatedBy = ValueOrDefault(row, "CreatedBy", SeedUser)
            })
            .ToList();
        await context.Products.AddRangeAsync(products);
        await context.SaveChangesAsync();

        var productImages = Sheet(workbook, "ProductImages")
            .Select(row => new ProductImage
            {
                Id = ParseGuid(row, "Id"),
                ProductId = ParseGuid(row, "ProductId"),
                FileName = GetValue(row, "FileName"),
                FileUrl = GetValue(row, "FileUrl"),
                ContentType = ValueOrNull(row, "ContentType"),
                FileSizeInBytes = LongValue(row, "FileSizeInBytes"),
                IsPrimary = BoolValue(row, "IsPrimary", false),
                DisplayOrder = IntValue(row, "DisplayOrder"),
                AltText = ValueOrNull(row, "AltText"),
                CreatedAtUtc = DateValue(row, "CreatedAtUtc", DateTime.UtcNow),
                UpdatedAtUtc = DateValueOrNull(row, "UpdatedAtUtc"),
                IsDeleted = BoolValue(row, "IsDeleted", false),
                DeletedAtUtc = DateValueOrNull(row, "DeletedAtUtc"),
                CreatedBy = ValueOrDefault(row, "CreatedBy", SeedUser)
            })
            .ToList();
        await context.ProductImages.AddRangeAsync(productImages);

        var inventoryStocks = Sheet(workbook, "InventoryStocks")
            .Select(row => new InventoryStock
            {
                Id = ParseGuid(row, "Id"),
                ProductId = ParseGuid(row, "ProductId"),
                WarehouseId = ParseGuid(row, "WarehouseId"),
                QuantityOnHand = DecimalValue(row, "QuantityOnHand"),
                QuantityReserved = DecimalValue(row, "QuantityReserved"),
                QuantityAvailable = DecimalValue(row, "QuantityAvailable"),
                QuantityDamaged = DecimalValue(row, "QuantityDamaged"),
                LastStockUpdatedAtUtc = DateValueOrNull(row, "LastStockUpdatedAtUtc"),
                CreatedAtUtc = DateValue(row, "CreatedAtUtc", DateTime.UtcNow),
                UpdatedAtUtc = DateValueOrNull(row, "UpdatedAtUtc"),
                IsDeleted = BoolValue(row, "IsDeleted", false),
                DeletedAtUtc = DateValueOrNull(row, "DeletedAtUtc"),
                CreatedBy = ValueOrDefault(row, "CreatedBy", SeedUser)
            })
            .ToList();
        await context.InventoryStocks.AddRangeAsync(inventoryStocks);

        var stockMovements = Sheet(workbook, "StockMovements")
            .Select(row => new StockMovement
            {
                Id = ParseGuid(row, "Id"),
                ProductId = ParseGuid(row, "ProductId"),
                WarehouseId = ParseGuid(row, "WarehouseId"),
                RelatedWarehouseId = GuidValueOrNull(row, "RelatedWarehouseId"),
                MovementType = EnumValue(row, "MovementType", StockMovementType.StockIn),
                Quantity = DecimalValue(row, "Quantity"),
                QuantityBefore = DecimalValue(row, "QuantityBefore"),
                QuantityAfter = DecimalValue(row, "QuantityAfter"),
                ReferenceNumber = ValueOrNull(row, "ReferenceNumber"),
                Reason = ValueOrNull(row, "Reason"),
                Notes = ValueOrNull(row, "Notes"),
                MovementDateUtc = DateValue(row, "MovementDateUtc", DateTime.UtcNow),
                CreatedAtUtc = DateValue(row, "CreatedAtUtc", DateTime.UtcNow),
                UpdatedAtUtc = DateValueOrNull(row, "UpdatedAtUtc"),
                IsDeleted = BoolValue(row, "IsDeleted", false),
                DeletedAtUtc = DateValueOrNull(row, "DeletedAtUtc"),
                CreatedBy = ValueOrDefault(row, "CreatedBy", SeedUser)
            })
            .ToList();
        await context.StockMovements.AddRangeAsync(stockMovements);

        var productSuppliers = Sheet(workbook, "ProductSuppliers")
            .Select(row => new ProductSupplier
            {
                Id = ParseGuid(row, "Id"),
                ProductId = ParseGuid(row, "ProductId"),
                SupplierId = ParseGuid(row, "SupplierId"),
                SupplierProductCode = ValueOrNull(row, "SupplierProductCode"),
                SupplierPrice = DecimalValue(row, "SupplierPrice"),
                LeadTimeInDays = IntValue(row, "LeadTimeInDays"),
                IsPreferredSupplier = BoolValue(row, "IsPreferredSupplier", false),
                CreatedAtUtc = DateValue(row, "CreatedAtUtc", DateTime.UtcNow),
                UpdatedAtUtc = DateValueOrNull(row, "UpdatedAtUtc"),
                IsDeleted = BoolValue(row, "IsDeleted", false),
                DeletedAtUtc = DateValueOrNull(row, "DeletedAtUtc"),
                CreatedBy = ValueOrDefault(row, "CreatedBy", SeedUser)
            })
            .ToList();
        await context.ProductSuppliers.AddRangeAsync(productSuppliers);

        await context.SaveChangesAsync();
        Console.WriteLine($"Imported inventory workbook: Products={products.Count}, Warehouses={warehouses.Count}, Stocks={inventoryStocks.Count}");
    }

    private static async Task SeedOrdersAsync(OperationIntelligenceDbContext context)
    {
        if (await context.Orders.AnyAsync())
        {
            Console.WriteLine("Order data already exists. Skipping order workbook import.");
            return;
        }

        EnsureWorkbookExists(OrdersWorkbookPath);
        var workbook = ReadWorkbookSheets(OrdersWorkbookPath);
        EnsureWorkbookExists(ShipmentWorkbookPath);
        var shipmentWorkbook = ReadWorkbookSheets(ShipmentWorkbookPath);
        var products = await context.Products.AsNoTracking().ToDictionaryAsync(product => product.Id);
        var defaultUom = await context.UnitsOfMeasure.AsNoTracking().FirstAsync();
        var warehouseMap = Sheet(shipmentWorkbook, "Normalization_WarehouseMap")
            .Where(row => ValueOrNull(row, "OrderWarehouseId") != null && GuidValueOrNull(row, "MappedInventoryWarehouseId").HasValue)
            .ToDictionary(
                row => GetValue(row, "OrderWarehouseId"),
                row => GuidValueOrNull(row, "MappedInventoryWarehouseId")!.Value,
                StringComparer.OrdinalIgnoreCase);
        var productMap = Sheet(shipmentWorkbook, "Normalization_ProductMap")
            .Where(row => ValueOrNull(row, "OrderProductId") != null && GuidValueOrNull(row, "MappedInventoryProductId").HasValue)
            .ToDictionary(
                row => GetValue(row, "OrderProductId"),
                row => new NormalizedProductMap(
                    GuidValueOrNull(row, "MappedInventoryProductId")!.Value,
                    GuidValueOrNull(row, "MappedUnitOfMeasureId")),
                StringComparer.OrdinalIgnoreCase);

        var orders = Sheet(workbook, "Orders")
            .Select(row =>
            {
                var paidAmount = DecimalValue(row, "PaidAmount");
                var totalAmount = DecimalValue(row, "TotalAmount");
                var rawWarehouseId = ValueOrNull(row, "WarehouseId");
                return new Order
                {
                    Id = ParseGuid(row, "Id"),
                    OrderNumber = GetValue(row, "OrderNumber"),
                    CustomerName = ValueOrNull(row, "CustomerName"),
                    CustomerEmail = ValueOrNull(row, "CustomerEmail"),
                    CustomerPhone = ValueOrNull(row, "CustomerPhone"),
                    OrderType = EnumValue(row, "OrderType", OrderType.Sales),
                    Status = EnumValue(row, "Status", OrderStatus.Draft),
                    Priority = EnumValue(row, "Priority", OrderPriority.Normal),
                    Channel = EnumValue(row, "Channel", OrderChannel.Internal),
                    WarehouseId = ResolveNormalizedWarehouseId(rawWarehouseId, warehouseMap),
                    OrderDateUtc = DateValue(row, "OrderDateUtc", DateTime.UtcNow),
                    RequiredDateUtc = DateValueOrNull(row, "RequiredDateUtc"),
                    SubtotalAmount = DecimalValue(row, "SubtotalAmount"),
                    DiscountAmount = DecimalValue(row, "DiscountAmount"),
                    TaxAmount = DecimalValue(row, "TaxAmount"),
                    ShippingAmount = DecimalValue(row, "ShippingAmount"),
                    TotalAmount = totalAmount,
                    PaidAmount = paidAmount,
                    OutstandingAmount = DecimalValue(row, "OutstandingAmount"),
                    PaymentStatus = DerivePaymentStatus(paidAmount, totalAmount),
                    CurrencyCode = ValueOrDefault(row, "CurrencyCode", "CAD"),
                    ReferenceNumber = ValueOrNull(row, "ReferenceNumber"),
                    Notes = ValueOrNull(row, "Notes"),
                    IsActive = BoolValue(row, "IsActive", true),
                    CreatedAtUtc = DateValue(row, "CreatedAtUtc", DateValue(row, "OrderDateUtc", DateTime.UtcNow)),
                    CreatedBy = ValueOrDefault(row, "CreatedBy", SeedUser)
                };
            })
            .ToList();
        await context.Orders.AddRangeAsync(orders);

        var orderItems = Sheet(workbook, "OrderItems")
            .Select(row =>
            {
                var normalizedProduct = ResolveNormalizedProduct(GetValue(row, "ProductId"), productMap);
                var productId = normalizedProduct.ProductId;
                products.TryGetValue(productId, out var product);
                var unitOfMeasureId = normalizedProduct.UnitOfMeasureId ?? product?.UnitOfMeasureId ?? defaultUom.Id;
                return new OrderItem
                {
                    Id = ParseGuid(row, "Id"),
                    OrderId = ParseGuid(row, "OrderId"),
                    ProductId = productId,
                    UnitOfMeasureId = unitOfMeasureId,
                    ProductNameSnapshot = ValueOrDefault(row, "ProductNameSnapshot", product?.Name ?? string.Empty),
                    ProductSkuSnapshot = ValueOrDefault(row, "ProductSkuSnapshot", product?.SKU ?? string.Empty),
                    QuantityOrdered = DecimalValue(row, "QuantityOrdered"),
                    UnitPrice = DecimalValue(row, "UnitPrice"),
                    DiscountAmount = DecimalValue(row, "DiscountAmount"),
                    TaxAmount = DecimalValue(row, "TaxAmount"),
                    LineTotal = DecimalValue(row, "LineTotal"),
                    IsActive = BoolValue(row, "IsActive", true),
                    CreatedAtUtc = DateValue(row, "CreatedAtUtc", DateTime.UtcNow),
                    CreatedBy = ValueOrDefault(row, "CreatedBy", SeedUser)
                };
            })
            .ToList();
        await context.OrderItems.AddRangeAsync(orderItems);

        var orderPayments = Sheet(workbook, "OrderPayments")
            .Select(row =>
            {
                var amount = DecimalValue(row, "Amount");
                return new OrderPayment
                {
                    Id = ParseGuid(row, "Id"),
                    OrderId = ParseGuid(row, "OrderId"),
                    PaymentReference = GetValue(row, "PaymentReference"),
                    PaymentMethod = EnumValue(row, "PaymentMethod", PaymentMethod.Other),
                    PaymentProvider = EnumValue(row, "PaymentProvider", PaymentProvider.Other),
                    TransactionType = EnumValue(row, "TransactionType", PaymentTransactionType.Payment),
                    Status = EnumValue(row, "Status", PaymentStatus.Pending),
                    Amount = amount,
                    NetAmount = amount,
                    CurrencyCode = ValueOrDefault(row, "CurrencyCode", "CAD"),
                    PaymentDateUtc = DateValue(row, "PaymentDateUtc", DateTime.UtcNow),
                    ProcessedDateUtc = DateValueOrNull(row, "PaymentDateUtc"),
                    RecordedBy = ValueOrDefault(row, "CreatedBy", SeedUser),
                    IsActive = BoolValue(row, "IsActive", true),
                    CreatedAtUtc = DateValue(row, "CreatedAtUtc", DateTime.UtcNow),
                    CreatedBy = ValueOrDefault(row, "CreatedBy", SeedUser)
                };
            })
            .ToList();
        await context.OrderPayments.AddRangeAsync(orderPayments);

        var orderImages = Sheet(workbook, "OrderImages")
            .Select(row =>
            {
                var fileName = GetValue(row, "FileName");
                return new OrderImage
                {
                    Id = ParseGuid(row, "Id"),
                    OrderId = ParseGuid(row, "OrderId"),
                    FileName = fileName,
                    OriginalFileName = fileName,
                    FileExtension = Path.GetExtension(fileName),
                    ContentType = ValueOrDefault(row, "ContentType", "application/octet-stream"),
                    FileSizeBytes = 0,
                    StoragePath = ValueOrDefault(row, "PublicUrl", $"/orders/images/{fileName}"),
                    PublicUrl = ValueOrNull(row, "PublicUrl"),
                    ImageType = EnumValue(row, "ImageType", OrderImageType.General),
                    Caption = ValueOrNull(row, "ImageType"),
                    UploadedAtUtc = DateValue(row, "UploadedAtUtc", DateTime.UtcNow),
                    UploadedBy = ValueOrDefault(row, "CreatedBy", SeedUser),
                    IsActive = BoolValue(row, "IsActive", true),
                    CreatedAtUtc = DateValue(row, "CreatedAtUtc", DateTime.UtcNow),
                    CreatedBy = ValueOrDefault(row, "CreatedBy", SeedUser)
                };
            })
            .ToList();
        await context.OrderImages.AddRangeAsync(orderImages);

        var orderAddresses = Sheet(workbook, "OrderAddresses")
            .Select(row => new OrderAddress
            {
                Id = ParseGuid(row, "Id"),
                OrderId = ParseGuid(row, "OrderId"),
                AddressType = EnumValue(row, "AddressType", AddressType.Shipping),
                ContactName = GetValue(row, "ContactName"),
                AddressLine1 = GetValue(row, "AddressLine1"),
                City = GetValue(row, "City"),
                StateOrProvince = ValueOrDefault(row, "State", string.Empty),
                PostalCode = GetValue(row, "PostalCode"),
                Country = GetValue(row, "Country"),
                PhoneNumber = ValueOrNull(row, "PhoneNumber"),
                CreatedAtUtc = DateValue(row, "CreatedAtUtc", DateTime.UtcNow),
                CreatedBy = ValueOrDefault(row, "CreatedBy", SeedUser)
            })
            .ToList();
        await context.OrderAddresses.AddRangeAsync(orderAddresses);

        var orderNotes = Sheet(workbook, "OrderNotes")
            .Select(row => new OrderNote
            {
                Id = ParseGuid(row, "Id"),
                OrderId = ParseGuid(row, "OrderId"),
                Note = GetValue(row, "Note"),
                IsInternal = BoolValue(row, "IsInternal", true),
                CreatedAtUtc = DateValue(row, "CreatedAtUtc", DateTime.UtcNow),
                CreatedBy = ValueOrDefault(row, "CreatedBy", SeedUser)
            })
            .ToList();
        await context.OrderNotes.AddRangeAsync(orderNotes);

        var statusHistories = Sheet(workbook, "OrderStatusHistories")
            .Select(row => new OrderStatusHistory
            {
                Id = ParseGuid(row, "Id"),
                OrderId = ParseGuid(row, "OrderId"),
                FromStatus = EnumValue(row, "FromStatus", OrderStatus.Draft),
                ToStatus = EnumValue(row, "ToStatus", OrderStatus.PendingApproval),
                Reason = ValueOrNull(row, "Reason"),
                ChangedBy = ValueOrNull(row, "ChangedBy"),
                ChangedAtUtc = DateValue(row, "ChangedAtUtc", DateTime.UtcNow),
                CreatedAtUtc = DateValue(row, "CreatedAtUtc", DateTime.UtcNow),
                CreatedBy = ValueOrDefault(row, "CreatedBy", SeedUser)
            })
            .ToList();
        await context.OrderStatusHistories.AddRangeAsync(statusHistories);

        await context.SaveChangesAsync();
        Console.WriteLine($"Imported order workbook: Orders={orders.Count}, Items={orderItems.Count}, Payments={orderPayments.Count}");
    }

    private static async Task SeedProductionAsync(OperationIntelligenceDbContext context)
    {
        if (await context.ProductionOrders.AnyAsync())
        {
            Console.WriteLine("Production data already exists. Skipping production workbook import.");
            return;
        }

        EnsureWorkbookExists(ProductionWorkbookPath);
        var workbook = ReadWorkbookSheets(ProductionWorkbookPath);
        EnsureWorkbookExists(ShipmentWorkbookPath);
        var shipmentWorkbook = ReadWorkbookSheets(ShipmentWorkbookPath);

        var products = await context.Products.AsNoTracking().ToDictionaryAsync(product => product.Id);
        var uoms = await context.UnitsOfMeasure.AsNoTracking().ToDictionaryAsync(uom => uom.Id);
        var users = await context.Users.AsNoTracking().ToDictionaryAsync(user => user.Id);
        var warehouses = await context.Warehouses.AsNoTracking().OrderBy(warehouse => warehouse.Name).ToListAsync();
        var normalizedProductMap = Sheet(shipmentWorkbook, "Normalization_ProductMap")
            .Where(row => ValueOrNull(row, "OrderProductId") != null && GuidValueOrNull(row, "MappedInventoryProductId").HasValue)
            .ToDictionary(
                row => GetValue(row, "OrderProductId"),
                row => new NormalizedProductMap(
                    GuidValueOrNull(row, "MappedInventoryProductId")!.Value,
                    GuidValueOrNull(row, "MappedUnitOfMeasureId")),
                StringComparer.OrdinalIgnoreCase);
        var productionWarehouseMap = BuildExternalGuidMap(
            Sheet(workbook, "WorkCenters").Select(row => GetValue(row, "WarehouseId"))
                .Concat(Sheet(workbook, "ProductionOrders").Select(row => GetValue(row, "WarehouseId")))
                .Concat(Sheet(workbook, "MaterialIssues").Select(row => GetValue(row, "WarehouseId")))
                .Concat(Sheet(workbook, "Outputs").Select(row => GetValue(row, "WarehouseId"))),
            warehouses.Select(warehouse => warehouse.Id));

        var workCenters = Sheet(workbook, "WorkCenters")
            .Select(row => new WorkCenter
            {
                Id = ParseGuid(row, "Id"),
                Code = GetValue(row, "Code"),
                Name = GetValue(row, "Name"),
                WarehouseId = ResolveMappedGuid(GetValue(row, "WarehouseId"), productionWarehouseMap),
                CapacityPerDay = 480m,
                AvailableOperators = 4,
                IsActive = BoolValue(row, "IsActive", true),
                CreatedAtUtc = DateValue(row, "CreatedAtUtc", DateTime.UtcNow),
                CreatedBy = ValueOrDefault(row, "CreatedBy", SeedUser)
            })
            .ToList();
        await context.WorkCenters.AddRangeAsync(workCenters);

        var machines = Sheet(workbook, "Machines")
            .Select(row => new Machine
            {
                Id = ParseGuid(row, "Id"),
                MachineCode = GetValue(row, "MachineCode"),
                Name = ValueOrDefault(row, "MachineName", GetValue(row, "MachineCode")),
                WorkCenterId = ParseGuid(row, "WorkCenterId"),
                Manufacturer = "Imported Seeder",
                HourlyRunningCost = 25m,
                Status = MachineStatus.Idle,
                IsActive = BoolValue(row, "IsActive", true),
                CreatedAtUtc = DateValue(row, "CreatedAtUtc", DateTime.UtcNow),
                CreatedBy = ValueOrDefault(row, "CreatedBy", SeedUser)
            })
            .ToList();
        await context.Machines.AddRangeAsync(machines);

        var boms = Sheet(workbook, "BOMs")
            .Select(row =>
            {
                var normalizedProduct = ResolveNormalizedProduct(GetValue(row, "ProductId"), normalizedProductMap);
                var productId = normalizedProduct.ProductId;
                return new BillOfMaterial
                {
                    Id = ParseGuid(row, "Id"),
                    BomCode = GetValue(row, "BomCode"),
                    Name = products.TryGetValue(productId, out var product) ? $"{product.Name} BOM" : GetValue(row, "BomCode"),
                    ProductId = productId,
                    BaseQuantity = DecimalValue(row, "BaseQuantity"),
                    UnitOfMeasureId = normalizedProduct.UnitOfMeasureId ?? ParseGuid(row, "UnitOfMeasureId"),
                    Version = IntValue(row, "Version"),
                    IsActive = BoolValue(row, "IsActive", true),
                    IsDefault = true,
                    EffectiveFrom = DateValue(row, "CreatedAtUtc", DateTime.UtcNow),
                    CreatedAtUtc = DateValue(row, "CreatedAtUtc", DateTime.UtcNow),
                    CreatedBy = ValueOrDefault(row, "CreatedBy", SeedUser)
                };
            })
            .ToList();
        await context.BillsOfMaterial.AddRangeAsync(boms);

        var bomItems = Sheet(workbook, "BOMItems")
            .Select(row =>
            {
                var normalizedProduct = ResolveNormalizedProduct(GetValue(row, "MaterialProductId"), normalizedProductMap);
                return new BillOfMaterialItem
                {
                    Id = ParseGuid(row, "Id"),
                    BillOfMaterialId = ParseGuid(row, "BillOfMaterialId"),
                    MaterialProductId = normalizedProduct.ProductId,
                    QuantityRequired = DecimalValue(row, "Quantity"),
                    Sequence = IntValue(row, "Sequence"),
                    UnitOfMeasureId = normalizedProduct.UnitOfMeasureId ?? ParseGuid(row, "UnitOfMeasureId"),
                    ScrapFactorPercent = 0m,
                    YieldFactorPercent = 100m,
                    CreatedAtUtc = DateValue(row, "CreatedAtUtc", DateTime.UtcNow),
                    CreatedBy = ValueOrDefault(row, "CreatedBy", SeedUser)
                };
            })
            .ToList();
        await context.BillOfMaterialItems.AddRangeAsync(bomItems);

        var routings = Sheet(workbook, "Routings")
            .Select(row =>
            {
                var productId = ResolveNormalizedProduct(GetValue(row, "ProductId"), normalizedProductMap).ProductId;
                return new Routing
                {
                    Id = ParseGuid(row, "Id"),
                    RoutingCode = GetValue(row, "RoutingCode"),
                    Name = products.TryGetValue(productId, out var product) ? $"{product.Name} Routing" : GetValue(row, "RoutingCode"),
                    ProductId = productId,
                    Version = IntValue(row, "Version"),
                    IsActive = BoolValue(row, "IsActive", true),
                    IsDefault = true,
                    EffectiveFrom = DateValue(row, "CreatedAtUtc", DateTime.UtcNow),
                    CreatedAtUtc = DateValue(row, "CreatedAtUtc", DateTime.UtcNow),
                    CreatedBy = ValueOrDefault(row, "CreatedBy", SeedUser)
                };
            })
            .ToList();
        await context.Routings.AddRangeAsync(routings);

        var routingSteps = Sheet(workbook, "RoutingSteps")
            .Select(row =>
            {
                var operationName = GetValue(row, "OperationName");
                var sequence = IntValue(row, "Sequence");
                return new RoutingStep
                {
                    Id = ParseGuid(row, "Id"),
                    RoutingId = ParseGuid(row, "RoutingId"),
                    Sequence = sequence,
                    OperationCode = BuildOperationCode(operationName, sequence),
                    OperationName = operationName,
                    WorkCenterId = ParseGuid(row, "WorkCenterId"),
                    SetupTimeMinutes = DecimalValue(row, "SetupMinutes"),
                    RunTimeMinutesPerUnit = DecimalValue(row, "RunMinutes"),
                    RequiredOperators = 1,
                    CreatedAtUtc = DateValue(row, "CreatedAtUtc", DateTime.UtcNow),
                    CreatedBy = ValueOrDefault(row, "CreatedBy", SeedUser)
                };
            })
            .ToList();
        await context.RoutingSteps.AddRangeAsync(routingSteps);

        var bomByProduct = boms.GroupBy(bom => bom.ProductId).ToDictionary(group => group.Key, group => group.First().Id);
        var routingByProduct = routings.GroupBy(routing => routing.ProductId).ToDictionary(group => group.Key, group => group.First().Id);

        var productionOrders = Sheet(workbook, "ProductionOrders")
            .Select(row =>
            {
                var normalizedProduct = ResolveNormalizedProduct(GetValue(row, "ProductId"), normalizedProductMap);
                var productId = normalizedProduct.ProductId;
                var plannedQty = DecimalValue(row, "PlannedQuantity");
                var status = EnumValue(row, "Status", ProductionOrderStatus.Planned);
                var producedQty = status == ProductionOrderStatus.Completed ? plannedQty : 0m;
                return new ProductionOrder
                {
                    Id = ParseGuid(row, "Id"),
                    ProductionOrderNumber = GetValue(row, "ProductionOrderNumber"),
                    ProductId = productId,
                    PlannedQuantity = plannedQty,
                    ProducedQuantity = producedQty,
                    ScrapQuantity = 0m,
                    RemainingQuantity = Math.Max(0m, plannedQty - producedQty),
                    UnitOfMeasureId = normalizedProduct.UnitOfMeasureId ?? ParseGuid(row, "UnitOfMeasureId"),
                    BillOfMaterialId = bomByProduct.TryGetValue(productId, out var bomId) ? bomId : null,
                    RoutingId = routingByProduct.TryGetValue(productId, out var routingId) ? routingId : null,
                    WarehouseId = ResolveMappedGuid(GetValue(row, "WarehouseId"), productionWarehouseMap),
                    PlannedStartDate = DateValue(row, "PlannedStartDate", DateTime.UtcNow),
                    PlannedEndDate = DateValue(row, "PlannedEndDate", DateTime.UtcNow.AddHours(8)),
                    Status = status,
                    Priority = ProductionPriority.Medium,
                    SourceType = ProductionSourceType.Manual,
                    IsReleased = status is ProductionOrderStatus.Released or ProductionOrderStatus.InProgress or ProductionOrderStatus.Completed or ProductionOrderStatus.Closed,
                    IsClosed = status is ProductionOrderStatus.Completed or ProductionOrderStatus.Closed,
                    CreatedAtUtc = DateValue(row, "CreatedAtUtc", DateTime.UtcNow),
                    CreatedBy = ValueOrDefault(row, "CreatedBy", SeedUser)
                };
            })
            .ToList();
        await context.ProductionOrders.AddRangeAsync(productionOrders);

        var orderById = productionOrders.ToDictionary(order => order.Id);
        var routingStepsByRouting = routingSteps.GroupBy(step => step.RoutingId).ToDictionary(group => group.Key, group => group.OrderBy(step => step.Sequence).ToList());

        var executions = Sheet(workbook, "Executions")
            .Select(row =>
            {
                var productionOrderId = ParseGuid(row, "ProductionOrderId");
                var productionOrder = orderById[productionOrderId];
                var start = productionOrder.PlannedStartDate;
                var end = productionOrder.PlannedEndDate;
                Guid? routingStepId = productionOrder.RoutingId.HasValue &&
                                      routingStepsByRouting.TryGetValue(productionOrder.RoutingId.Value, out var steps)
                    ? steps.FirstOrDefault(step => step.WorkCenterId == ParseGuid(row, "WorkCenterId"))?.Id ?? steps.First().Id
                    : null;

                return new ProductionExecution
                {
                    Id = ParseGuid(row, "Id"),
                    ProductionOrderId = productionOrderId,
                    RoutingStepId = routingStepId,
                    WorkCenterId = ParseGuid(row, "WorkCenterId"),
                    MachineId = GuidValueOrNull(row, "MachineId"),
                    PlannedQuantity = DecimalValue(row, "PlannedQuantity"),
                    CompletedQuantity = DecimalValue(row, "CompletedQuantity"),
                    PlannedStartDate = start,
                    PlannedEndDate = end,
                    Status = EnumValue(row, "Status", ExecutionStatus.Pending),
                    CreatedAtUtc = DateValue(row, "CreatedAtUtc", DateTime.UtcNow),
                    CreatedBy = ValueOrDefault(row, "CreatedBy", SeedUser)
                };
            })
            .ToList();
        await context.ProductionExecutions.AddRangeAsync(executions);

        var executionsByOrder = executions.GroupBy(execution => execution.ProductionOrderId)
            .ToDictionary(group => group.Key, group => group.OrderBy(execution => execution.CreatedAtUtc).ToList());

        var materialIssues = Sheet(workbook, "MaterialIssues")
            .Select(row =>
            {
                var normalizedProduct = ResolveNormalizedProduct(GetValue(row, "MaterialProductId"), normalizedProductMap);
                var materialProductId = normalizedProduct.ProductId;
                var issueDate = DateValue(row, "CreatedAtUtc", DateTime.UtcNow);
                return new ProductionMaterialIssue
                {
                    Id = ParseGuid(row, "Id"),
                    ProductionOrderId = ParseGuid(row, "ProductionOrderId"),
                    MaterialProductId = materialProductId,
                    IssuedQuantity = DecimalValue(row, "IssuedQuantity"),
                    PlannedQuantity = DecimalValue(row, "IssuedQuantity"),
                    WarehouseId = ResolveMappedGuid(GetValue(row, "WarehouseId"), productionWarehouseMap),
                    UnitOfMeasureId = normalizedProduct.UnitOfMeasureId ??
                                      (products.TryGetValue(materialProductId, out var product) ? product.UnitOfMeasureId : uoms.Values.First().Id),
                    IssueDate = issueDate,
                    CreatedAtUtc = issueDate,
                    CreatedBy = ValueOrDefault(row, "CreatedBy", SeedUser)
                };
            })
            .ToList();
        await context.ProductionMaterialIssues.AddRangeAsync(materialIssues);

        var materialIssueById = materialIssues.ToDictionary(issue => issue.Id);
        var consumptions = Sheet(workbook, "MaterialConsumptions")
            .Select(row =>
            {
                var issueId = ParseGuid(row, "ProductionMaterialIssueId");
                var issue = materialIssueById[issueId];
                return new ProductionMaterialConsumption
                {
                    Id = ParseGuid(row, "Id"),
                    ProductionMaterialIssueId = issueId,
                    ProductionExecutionId = executionsByOrder.TryGetValue(issue.ProductionOrderId, out var relatedExecutions)
                        ? relatedExecutions.FirstOrDefault()?.Id
                        : null,
                    ConsumedQuantity = DecimalValue(row, "ConsumedQuantity"),
                    ConsumptionDate = DateValue(row, "CreatedAtUtc", DateTime.UtcNow),
                    CreatedAtUtc = DateValue(row, "CreatedAtUtc", DateTime.UtcNow),
                    CreatedBy = ValueOrDefault(row, "CreatedBy", SeedUser)
                };
            })
            .ToList();
        await context.ProductionMaterialConsumptions.AddRangeAsync(consumptions);

        var outputs = Sheet(workbook, "Outputs")
            .Select(row =>
            {
                var productionOrderId = ParseGuid(row, "ProductionOrderId");
                var productionOrder = orderById[productionOrderId];
                return new ProductionOutput
                {
                    Id = ParseGuid(row, "Id"),
                    ProductionOrderId = productionOrderId,
                    ProductId = productionOrder.ProductId,
                    WarehouseId = ResolveMappedGuid(GetValue(row, "WarehouseId"), productionWarehouseMap),
                    QuantityProduced = DecimalValue(row, "QuantityProduced"),
                    UnitOfMeasureId = productionOrder.UnitOfMeasureId,
                    OutputDate = DateValue(row, "CreatedAtUtc", DateTime.UtcNow),
                    CreatedAtUtc = DateValue(row, "CreatedAtUtc", DateTime.UtcNow),
                    CreatedBy = ValueOrDefault(row, "CreatedBy", SeedUser)
                };
            })
            .ToList();
        await context.ProductionOutputs.AddRangeAsync(outputs);

        var scraps = Sheet(workbook, "Scrap")
            .Select(row =>
            {
                var productionOrderId = ParseGuid(row, "ProductionOrderId");
                var productionOrder = orderById[productionOrderId];
                return new ProductionScrap
                {
                    Id = ParseGuid(row, "Id"),
                    ProductionOrderId = productionOrderId,
                    ProductionExecutionId = executionsByOrder.TryGetValue(productionOrderId, out var relatedExecutions)
                        ? relatedExecutions.FirstOrDefault()?.Id
                        : null,
                    ProductId = productionOrder.ProductId,
                    ScrapQuantity = DecimalValue(row, "ScrapQuantity"),
                    UnitOfMeasureId = productionOrder.UnitOfMeasureId,
                    Reason = ScrapReasonType.Other,
                    ReasonDescription = ValueOrNull(row, "Reason"),
                    ScrapDate = DateValue(row, "CreatedAtUtc", DateTime.UtcNow),
                    CreatedAtUtc = DateValue(row, "CreatedAtUtc", DateTime.UtcNow),
                    CreatedBy = ValueOrDefault(row, "CreatedBy", SeedUser)
                };
            })
            .ToList();
        await context.ProductionScraps.AddRangeAsync(scraps);

        var executionById = executions.ToDictionary(execution => execution.Id);
        var downtimes = Sheet(workbook, "Downtime")
            .Select(row =>
            {
                var createdAt = DateValue(row, "CreatedAtUtc", DateTime.UtcNow);
                var duration = DecimalValue(row, "DurationMinutes");
                return new ProductionDowntime
                {
                    Id = ParseGuid(row, "Id"),
                    ProductionExecutionId = ParseGuid(row, "ProductionExecutionId"),
                    Reason = DowntimeReasonType.Other,
                    ReasonDescription = ValueOrNull(row, "Reason"),
                    StartTime = createdAt,
                    EndTime = createdAt.AddMinutes((double)duration),
                    DurationMinutes = duration,
                    IsPlanned = false,
                    CreatedAtUtc = createdAt,
                    CreatedBy = ValueOrDefault(row, "CreatedBy", SeedUser)
                };
            })
            .ToList();
        await context.ProductionDowntimes.AddRangeAsync(downtimes);

        var laborLogs = Sheet(workbook, "LaborLogs")
            .Select(row => new ProductionLaborLog
            {
                Id = ParseGuid(row, "Id"),
                ProductionExecutionId = ParseGuid(row, "ProductionExecutionId"),
                UserId = ParseGuid(row, "UserId"),
                HoursWorked = DecimalValue(row, "HoursWorked"),
                HourlyRate = 30m,
                WorkDate = DateValue(row, "CreatedAtUtc", DateTime.UtcNow),
                CreatedAtUtc = DateValue(row, "CreatedAtUtc", DateTime.UtcNow),
                CreatedBy = ValueOrDefault(row, "CreatedBy", SeedUser)
            })
            .Where(log => users.ContainsKey(log.UserId))
            .ToList();
        await context.ProductionLaborLogs.AddRangeAsync(laborLogs);

        var qualityChecks = Sheet(workbook, "QualityChecks")
            .Select(row =>
            {
                var status = EnumValue(row, "Status", QualityCheckStatus.Pending);
                return new ProductionQualityCheck
                {
                    Id = ParseGuid(row, "Id"),
                    ProductionOrderId = ParseGuid(row, "ProductionOrderId"),
                    ProductionExecutionId = executionsByOrder.TryGetValue(ParseGuid(row, "ProductionOrderId"), out var relatedExecutions)
                        ? relatedExecutions.FirstOrDefault()?.Id
                        : null,
                    CheckType = QualityCheckType.Final,
                    Status = status,
                    CheckDate = DateValue(row, "CheckDate", DateTime.UtcNow),
                    CheckedByUserId = ParseGuid(row, "CheckedByUserId"),
                    RequiresRework = status == QualityCheckStatus.Failed,
                    CreatedAtUtc = DateValue(row, "CreatedAtUtc", DateTime.UtcNow),
                    CreatedBy = ValueOrDefault(row, "CreatedBy", SeedUser)
                };
            })
            .Where(check => users.ContainsKey(check.CheckedByUserId))
            .ToList();
        await context.ProductionQualityChecks.AddRangeAsync(qualityChecks);

        await context.SaveChangesAsync();
        Console.WriteLine($"Imported production workbook: WorkCenters={workCenters.Count}, Orders={productionOrders.Count}, Executions={executions.Count}");
    }

    private static async Task SeedSchedulingAsync(OperationIntelligenceDbContext context)
    {
        if (await context.SchedulePlans.AnyAsync())
        {
            Console.WriteLine("Scheduling data already exists. Skipping scheduling workbook import.");
            return;
        }

        EnsureWorkbookExists(SchedulingWorkbookPath);
        var workbook = ReadWorkbookSheets(SchedulingWorkbookPath);

        var warehouses = await context.Warehouses.AsNoTracking().OrderBy(warehouse => warehouse.Name).ToListAsync();
        var orders = await context.Orders.AsNoTracking().Include(order => order.Items).ToDictionaryAsync(order => order.Id);
        var products = await context.Products.AsNoTracking().ToListAsync();
        var workCenters = await context.WorkCenters.AsNoTracking().ToDictionaryAsync(workCenter => workCenter.Id);
        var machines = await context.Machines.AsNoTracking().ToDictionaryAsync(machine => machine.Id);
        var productionOrders = await context.ProductionOrders.AsNoTracking().ToListAsync();
        var routings = await context.Routings.AsNoTracking().ToListAsync();
        var routingSteps = await context.RoutingSteps.AsNoTracking().ToListAsync();
        var materialIssues = await context.ProductionMaterialIssues.AsNoTracking().ToListAsync();
        EnsureWorkbookExists(ShipmentWorkbookPath);
        var shipmentWorkbook = ReadWorkbookSheets(ShipmentWorkbookPath);
        var normalizedProductMap = Sheet(shipmentWorkbook, "Normalization_ProductMap")
            .Where(row => ValueOrNull(row, "OrderProductId") != null && GuidValueOrNull(row, "MappedInventoryProductId").HasValue)
            .ToDictionary(
                row => GetValue(row, "OrderProductId"),
                row => new NormalizedProductMap(
                    GuidValueOrNull(row, "MappedInventoryProductId")!.Value,
                    GuidValueOrNull(row, "MappedUnitOfMeasureId")),
                StringComparer.OrdinalIgnoreCase);
        var schedulePlanIdMap = CreateLocalIdMap(Sheet(workbook, "SchedulePlans").Select(row => GetValue(row, "Id")));
        var scheduleJobIdMap = CreateLocalIdMap(Sheet(workbook, "ScheduleJobs").Select(row => GetValue(row, "Id")));
        var scheduleOperationIdMap = CreateLocalIdMap(Sheet(workbook, "ScheduleOperations").Select(row => GetValue(row, "Id")));
        var shiftIdMap = CreateLocalIdMap(Sheet(workbook, "Shifts").Select(row => GetValue(row, "Id")));
        var scheduleWarehouseMap = BuildExternalGuidMap(
            Sheet(workbook, "SchedulePlans").Select(row => GetValue(row, "WarehouseId")),
            warehouses.Select(warehouse => warehouse.Id));

        var plans = Sheet(workbook, "SchedulePlans")
            .Select(row => new SchedulePlan
            {
                Id = ResolveMappedGuid(GetValue(row, "Id"), schedulePlanIdMap),
                PlanNumber = GetValue(row, "PlanNumber"),
                Name = GetValue(row, "Name"),
                Description = $"Imported plan {GetValue(row, "PlanNumber")}",
                WarehouseId = ResolveMappedGuid(GetValue(row, "WarehouseId"), scheduleWarehouseMap),
                PlanningStartDateUtc = DateValue(row, "PlanningStartDateUtc", DateTime.UtcNow),
                PlanningEndDateUtc = DateValue(row, "PlanningEndDateUtc", DateTime.UtcNow.AddDays(7)),
                Status = EnumValue(row, "Status", SchedulePlanStatus.Draft),
                GenerationMode = ScheduleGenerationMode.Manual,
                SchedulingStrategy = SchedulingStrategy.FiniteCapacity,
                AutoSequenceEnabled = true,
                AutoDispatchEnabled = false,
                VersionNumber = IntValue(row, "VersionNumber"),
                TimeZone = "UTC",
                IsActive = true,
                CreatedAtUtc = DateValue(row, "CreatedAtUtc", DateTime.UtcNow),
                CreatedBy = ValueOrDefault(row, "CreatedBy", SeedUser)
            })
            .ToList();
        await context.SchedulePlans.AddRangeAsync(plans);

        var shiftRows = Sheet(workbook, "Shifts");
        var shifts = shiftRows
            .Select(row =>
            {
                var workCenterId = ParseGuid(row, "WorkCenterId");
                var workCenter = workCenters[workCenterId];
                var startTime = TimeValue(row, "StartTime");
                var endTime = TimeValue(row, "EndTime");
                var crossesMidnight = endTime <= startTime;
                var durationMinutes = CalculateShiftMinutes(startTime, endTime, crossesMidnight);
                return new Shift
                {
                    Id = ResolveMappedGuid(GetValue(row, "Id"), shiftIdMap),
                    WarehouseId = workCenter.WarehouseId,
                    WorkCenterId = workCenterId,
                    ShiftCode = BuildShiftCode(GetValue(row, "ShiftName")),
                    ShiftName = GetValue(row, "ShiftName"),
                    StartTime = startTime,
                    EndTime = endTime,
                    CrossesMidnight = crossesMidnight,
                    IsActive = BoolValue(row, "IsActive", true),
                    CapacityMinutes = durationMinutes,
                    BreakMinutes = durationMinutes >= 480 ? 30 : 15,
                    CreatedAtUtc = DateTime.UtcNow,
                    CreatedBy = SeedUser
                };
            })
            .ToList();
        await context.Shifts.AddRangeAsync(shifts);

        var productByNormalizedName = products
            .GroupBy(product => NormalizeLookup(product.Name))
            .ToDictionary(group => group.Key, group => group.First());

        var routingByProductId = routings.GroupBy(routing => routing.ProductId).ToDictionary(group => group.Key, group => group.ToList());
        var routingStepsByRoutingId = routingSteps.GroupBy(step => step.RoutingId).ToDictionary(group => group.Key, group => group.OrderBy(step => step.Sequence).ToList());
        var plansById = plans.ToDictionary(plan => plan.Id);

        var jobs = new List<ScheduleJob>();
        var jobMap = new Dictionary<Guid, ScheduleJob>();

        foreach (var row in Sheet(workbook, "ScheduleJobs"))
        {
            var jobId = ResolveMappedGuid(GetValue(row, "Id"), scheduleJobIdMap);
            var schedulePlanId = ResolveMappedGuid(GetValue(row, "SchedulePlanId"), schedulePlanIdMap);
            var plan = plansById[schedulePlanId];
            var orderId = GuidValueOrNull(row, "OrderId");
            var order = orderId.HasValue && orders.TryGetValue(orderId.Value, out var matchedOrder) ? matchedOrder : null;
            var product = ResolveProductForScheduleJob(row, order, productByNormalizedName, products);
            var orderItem = order?.Items.FirstOrDefault(item => item.ProductId == product.Id) ?? order?.Items.FirstOrDefault();
            var productionOrder = ResolveProductionOrder(product.Id, plan.WarehouseId, DecimalValue(row, "PlannedQuantity"), productionOrders);
            if (productionOrder == null)
            {
                throw new InvalidOperationException($"Unable to resolve production order for schedule job {jobId}.");
            }

            var priority = EnumValue(row, "Priority", SchedulePriority.Normal);
            var job = new ScheduleJob
            {
                Id = jobId,
                SchedulePlanId = schedulePlanId,
                ProductionOrderId = productionOrder.Id,
                OrderId = order?.Id,
                OrderItemId = orderItem?.Id,
                ProductId = product.Id,
                WarehouseId = plan.WarehouseId,
                JobNumber = ValueOrDefault(row, "OrderNumber", $"JOB-{jobId.ToString()[..8]}"),
                JobName = ValueOrDefault(row, "ProductName", product.Name),
                PlannedQuantity = DecimalValue(row, "PlannedQuantity"),
                CompletedQuantity = productionOrder.ProducedQuantity,
                ScrappedQuantity = productionOrder.ScrapQuantity,
                EarliestStartUtc = plan.PlanningStartDateUtc,
                LatestFinishUtc = DateValueOrNull(row, "DueDateUtc"),
                DueDateUtc = DateValueOrNull(row, "DueDateUtc"),
                Priority = priority,
                Status = ResolveScheduleJobStatus(ValueOrNull(row, "Status")),
                MaterialsReady = materialIssues.Any(issue => issue.ProductionOrderId == productionOrder.Id),
                MaterialReadinessStatus = MaterialReadinessStatus.NotChecked,
                IsRushOrder = priority is SchedulePriority.Urgent or SchedulePriority.Critical,
                CreatedAtUtc = DateValue(row, "CreatedAtUtc", DateTime.UtcNow),
                CreatedBy = ValueOrDefault(row, "CreatedBy", SeedUser)
            };

            jobs.Add(job);
            jobMap[jobId] = job;
        }

        await context.ScheduleJobs.AddRangeAsync(jobs);

        var executionsByProductionOrderId = await context.ProductionExecutions.AsNoTracking()
            .GroupBy(execution => execution.ProductionOrderId)
            .ToDictionaryAsync(group => group.Key, group => group.OrderBy(execution => execution.PlannedStartDate).ToList());
        var shiftsByWorkCenter = shifts.Where(shift => shift.WorkCenterId.HasValue)
            .GroupBy(shift => shift.WorkCenterId!.Value)
            .ToDictionary(group => group.Key, group => group.ToList());

        var operations = new List<ScheduleOperation>();
        var operationMap = new Dictionary<Guid, ScheduleOperation>();

        foreach (var row in Sheet(workbook, "ScheduleOperations"))
        {
            var operationId = ResolveMappedGuid(GetValue(row, "Id"), scheduleOperationIdMap);
            var scheduleJobId = ResolveMappedGuid(GetValue(row, "ScheduleJobId"), scheduleJobIdMap);
            var job = jobMap[scheduleJobId];
            var workCenterId = ParseGuid(row, "WorkCenterId");
            var machineId = GuidValueOrNull(row, "MachineId");
            var operationName = GetValue(row, "OperationName");
            var routingStep = ResolveRoutingStep(job.ProductId, workCenterId, operationName, IntValue(row, "SequenceNo"), routingByProductId, routingStepsByRoutingId, routingSteps);
            var plannedStart = DateValue(row, "PlannedStartUtc", job.EarliestStartUtc ?? DateTime.UtcNow);
            var plannedEnd = DateValue(row, "PlannedEndUtc", plannedStart.AddHours(1));
            var productionExecution = ResolveExecution(job.ProductionOrderId, workCenterId, machineId, executionsByProductionOrderId);
            var plannedShift = ResolveShiftForDate(workCenterId, plannedStart, shiftsByWorkCenter);
            var status = EnumValue(row, "Status", ScheduleOperationStatus.Pending);

            var operation = new ScheduleOperation
            {
                Id = operationId,
                ScheduleJobId = scheduleJobId,
                RoutingStepId = routingStep.Id,
                WorkCenterId = workCenterId,
                MachineId = machineId,
                ProductionExecutionId = productionExecution?.Id,
                PlannedShiftId = plannedShift?.Id,
                SequenceNo = IntValue(row, "SequenceNo"),
                OperationCode = routingStep.OperationCode,
                OperationName = operationName,
                PlannedStartUtc = plannedStart,
                PlannedEndUtc = plannedEnd,
                SetupTimeMinutes = routingStep.SetupTimeMinutes,
                RunTimeMinutes = routingStep.RunTimeMinutesPerUnit * Math.Max(job.PlannedQuantity, 1m),
                QueueTimeMinutes = routingStep.QueueTimeMinutes,
                WaitTimeMinutes = routingStep.WaitTimeMinutes,
                MoveTimeMinutes = routingStep.MoveTimeMinutes,
                PlannedQuantity = job.PlannedQuantity,
                CompletedQuantity = productionExecution?.CompletedQuantity ?? 0m,
                ScrappedQuantity = 0m,
                Status = status,
                DispatchStatus = MapDispatchStatus(status),
                IsCriticalPath = IntValue(row, "SequenceNo") == 1,
                IsBottleneckOperation = false,
                IsOutsourced = routingStep.IsOutsourced,
                PriorityScore = CalculatePriorityScore(job.Priority, IntValue(row, "SequenceNo")),
                ConstraintReason = ValueOrDefault(row, "Status", null),
                Notes = "Imported from workbook",
                CreatedAtUtc = plannedStart,
                CreatedBy = SeedUser
            };

            operations.Add(operation);
            operationMap[operationId] = operation;
        }

        await context.ScheduleOperations.AddRangeAsync(operations);

        var operationDependencies = Sheet(workbook, "OperationDependencies")
            .Where(row =>
            {
                var predecessor = ResolveOptionalMappedGuid(ValueOrNull(row, "PredecessorOperationId"), scheduleOperationIdMap);
                var successor = ResolveOptionalMappedGuid(ValueOrNull(row, "SuccessorOperationId"), scheduleOperationIdMap);
                return predecessor.HasValue && successor.HasValue &&
                       operationMap.ContainsKey(predecessor.Value) && operationMap.ContainsKey(successor.Value);
            })
            .Select(row => new ScheduleOperationDependency
            {
                Id = CreateStableGuid("sched-dep", GetValue(row, "Id")),
                PredecessorOperationId = ResolveMappedGuid(GetValue(row, "PredecessorOperationId"), scheduleOperationIdMap),
                SuccessorOperationId = ResolveMappedGuid(GetValue(row, "SuccessorOperationId"), scheduleOperationIdMap),
                DependencyType = EnumValue(row, "DependencyType", DependencyType.FinishToStart),
                CreatedAtUtc = DateTime.UtcNow,
                CreatedBy = SeedUser
            })
            .ToList();
        await context.ScheduleOperationDependencies.AddRangeAsync(operationDependencies);

        var resourceAssignments = Sheet(workbook, "ResourceAssignments")
            .Where(row => operationMap.ContainsKey(ResolveMappedGuid(GetValue(row, "ScheduleOperationId"), scheduleOperationIdMap)))
            .Select(row =>
            {
                var assignedStart = DateValue(row, "AssignedStartUtc", DateTime.UtcNow);
                var assignedEnd = DateValue(row, "AssignedEndUtc", assignedStart.AddHours(1));
                var op = operationMap[ResolveMappedGuid(GetValue(row, "ScheduleOperationId"), scheduleOperationIdMap)];
                var resourceType = ResolveResourceType(ValueOrNull(row, "ResourceType"));
                return new ScheduleResourceAssignment
                {
                    Id = CreateStableGuid("sched-assign", GetValue(row, "Id")),
                    ScheduleOperationId = op.Id,
                    ResourceId = ParseGuid(row, "ResourceId"),
                    ResourceType = resourceType,
                    ShiftId = ResolveShiftForDate(op.WorkCenterId, assignedStart, shiftsByWorkCenter)?.Id,
                    AssignmentRole = ValueOrDefault(row, "ResourceType", resourceType.ToString()),
                    IsPrimary = true,
                    AssignedStartUtc = assignedStart,
                    AssignedEndUtc = assignedEnd,
                    PlannedHours = Math.Max(0m, (decimal)(assignedEnd - assignedStart).TotalHours),
                    ActualHours = 0m,
                    Status = op.Status is ScheduleOperationStatus.Completed or ScheduleOperationStatus.Running
                        ? AssignmentStatus.Confirmed
                        : AssignmentStatus.Planned,
                    Notes = "Imported from workbook",
                    CreatedAtUtc = assignedStart,
                    CreatedBy = SeedUser
                };
            })
            .ToList();
        await context.ScheduleResourceAssignments.AddRangeAsync(resourceAssignments);

        var dispatchItems = Sheet(workbook, "DispatchQueue")
            .Where(row => operationMap.ContainsKey(ResolveMappedGuid(GetValue(row, "ScheduleOperationId"), scheduleOperationIdMap)))
            .Select(row =>
            {
                var op = operationMap[ResolveMappedGuid(GetValue(row, "ScheduleOperationId"), scheduleOperationIdMap)];
                return new DispatchQueueItem
                {
                    Id = CreateStableGuid("sched-dispatch", GetValue(row, "Id")),
                    ScheduleOperationId = op.Id,
                    WorkCenterId = op.WorkCenterId,
                    MachineId = op.MachineId,
                    QueuePosition = IntValue(row, "QueuePosition"),
                    PriorityScore = IntValue(row, "PriorityScore"),
                    Status = ResolveDispatchStatus(ValueOrNull(row, "Status")),
                    DispatchNotes = "Imported from workbook",
                    IsActive = true,
                    CreatedAtUtc = DateTime.UtcNow,
                    CreatedBy = SeedUser
                };
            })
            .ToList();
        await context.DispatchQueueItems.AddRangeAsync(dispatchItems);

        var materialChecks = Sheet(workbook, "MaterialChecks")
            .Where(row => operationMap.ContainsKey(ResolveMappedGuid(GetValue(row, "ScheduleOperationId"), scheduleOperationIdMap)))
            .Select(row =>
            {
                var op = operationMap[ResolveMappedGuid(GetValue(row, "ScheduleOperationId"), scheduleOperationIdMap)];
                var job = jobMap[op.ScheduleJobId];
                var mappedMaterialProductId = ResolveScheduleMaterialProductId(
                    ValueOrNull(row, "MaterialProductId"),
                    job.ProductionOrderId,
                    normalizedProductMap,
                    materialIssues,
                    products);
                return new ScheduleMaterialCheck
                {
                    Id = CreateStableGuid("sched-matcheck", GetValue(row, "Id")),
                    ScheduleJobId = job.Id,
                    ScheduleOperationId = op.Id,
                    ProductionOrderId = job.ProductionOrderId,
                    RoutingStepId = op.RoutingStepId,
                    MaterialProductId = mappedMaterialProductId,
                    WarehouseId = job.WarehouseId,
                    RequiredQuantity = DecimalValue(row, "RequiredQuantity"),
                    AvailableQuantity = DecimalValue(row, "AvailableQuantity"),
                    ReservedQuantity = Math.Min(DecimalValue(row, "RequiredQuantity"), DecimalValue(row, "AvailableQuantity")),
                    ShortageQuantity = Math.Max(0m, DecimalValue(row, "RequiredQuantity") - DecimalValue(row, "AvailableQuantity")),
                    Status = EnumValue(row, "Status", MaterialReadinessStatus.NotChecked),
                    CheckedAtUtc = DateTime.UtcNow,
                    CreatedAtUtc = DateTime.UtcNow,
                    CreatedBy = SeedUser
                };
            })
            .ToList();
        await context.ScheduleMaterialChecks.AddRangeAsync(materialChecks);

        var exceptions = Sheet(workbook, "Exceptions")
            .Where(row => operationMap.ContainsKey(ResolveMappedGuid(GetValue(row, "ScheduleOperationId"), scheduleOperationIdMap)))
            .Select(row =>
            {
                var op = operationMap[ResolveMappedGuid(GetValue(row, "ScheduleOperationId"), scheduleOperationIdMap)];
                return new ScheduleException
                {
                    Id = CreateStableGuid("sched-ex", GetValue(row, "Id")),
                    SchedulePlanId = jobMap[op.ScheduleJobId].SchedulePlanId,
                    ScheduleJobId = op.ScheduleJobId,
                    ScheduleOperationId = op.Id,
                    ExceptionType = EnumValue(row, "ExceptionType", ScheduleExceptionType.ManualOverride),
                    Severity = EnumValue(row, "Severity", ScheduleExceptionSeverity.Medium),
                    Title = $"{GetValue(row, "ExceptionType")} on {op.OperationName}",
                    Description = $"Imported exception for schedule operation {op.OperationName}.",
                    DetectedAtUtc = DateValue(row, "DetectedAtUtc", DateTime.UtcNow),
                    Status = EnumValue(row, "Status", ScheduleExceptionStatus.Open),
                    CreatedAtUtc = DateTime.UtcNow,
                    CreatedBy = SeedUser
                };
            })
            .ToList();
        await context.ScheduleExceptions.AddRangeAsync(exceptions);

        var revisions = Sheet(workbook, "Revisions")
            .Select(row =>
            {
                var planId = ResolveMappedGuid(GetValue(row, "SchedulePlanId"), schedulePlanIdMap);
                return new ScheduleRevision
                {
                    Id = CreateStableGuid("sched-rev", GetValue(row, "Id")),
                    SchedulePlanId = planId,
                    RevisionNo = IntValue(row, "RevisionNo"),
                    RevisionType = "Imported",
                    ChangeSummary = $"Imported revision for plan {plansById[planId].PlanNumber}",
                    Reason = ValueOrDefault(row, "Reason", "Imported from workbook"),
                    RevisedAtUtc = DateValue(row, "RevisedAtUtc", DateTime.UtcNow),
                    CreatedAtUtc = DateTime.UtcNow,
                    CreatedBy = SeedUser
                };
            })
            .ToList();
        await context.ScheduleRevisions.AddRangeAsync(revisions);

        var auditLogs = Sheet(workbook, "AuditLogs")
            .Select(row =>
            {
                var entityName = GetValue(row, "EntityName");
                var resolvedEntityId = ResolveScheduleAuditEntityId(entityName, plans, jobs, operations);
                return new ScheduleAuditLog
                {
                    Id = CreateStableGuid("sched-audit", GetValue(row, "Id")),
                    EntityName = entityName,
                    EntityId = resolvedEntityId,
                    ActionType = GetValue(row, "ActionType"),
                    ChangedFieldsJson = "[]",
                    Source = "WorkbookSeeder",
                    Reason = $"Imported audit event for user {ValueOrDefault(row, "UserId", "unknown")}",
                    PerformedAtUtc = DateValue(row, "PerformedAtUtc", DateTime.UtcNow),
                    CreatedAtUtc = DateTime.UtcNow,
                    CreatedBy = SeedUser
                };
            })
            .ToList();
        await context.ScheduleAuditLogs.AddRangeAsync(auditLogs);

        await context.SaveChangesAsync();
        Console.WriteLine($"Imported scheduling workbook: Plans={plans.Count}, Jobs={jobs.Count}, Operations={operations.Count}");
    }

    private static async Task SeedShipmentAsync(OperationIntelligenceDbContext context)
    {
        if (await context.Shipments.AnyAsync())
        {
            Console.WriteLine("Shipment data already exists. Skipping shipment workbook import.");
            return;
        }

        EnsureWorkbookExists(ShipmentWorkbookPath);
        var workbook = ReadWorkbookSheets(ShipmentWorkbookPath);

        var carriers = Sheet(workbook, "Carriers")
            .Select(row => new Carrier
            {
                Id = ParseGuid(row, "Id"),
                CarrierCode = GetValue(row, "CarrierCode"),
                Name = GetValue(row, "Name"),
                ContactName = ValueOrNull(row, "ContactName"),
                Phone = ValueOrNull(row, "Phone"),
                Email = ValueOrNull(row, "Email"),
                Website = ValueOrNull(row, "Website"),
                IsActive = BoolValue(row, "IsActive", true),
                CreatedAtUtc = DateValue(row, "CreatedAtUtc", DateTime.UtcNow),
                CreatedBy = ValueOrDefault(row, "CreatedBy", SeedUser)
            })
            .ToList();
        await context.Carriers.AddRangeAsync(carriers);

        var carrierServices = Sheet(workbook, "CarrierServices")
            .Select(row => new CarrierService
            {
                Id = ParseGuid(row, "Id"),
                CarrierId = ParseGuid(row, "CarrierId"),
                ServiceCode = GetValue(row, "ServiceCode"),
                Name = GetValue(row, "Name"),
                Description = ValueOrNull(row, "Description"),
                EstimatedTransitDays = IntValue(row, "EstimatedTransitDays"),
                IsActive = BoolValue(row, "IsActive", true),
                CreatedAtUtc = DateValue(row, "CreatedAtUtc", DateTime.UtcNow),
                CreatedBy = ValueOrDefault(row, "CreatedBy", SeedUser)
            })
            .ToList();
        await context.CarrierServices.AddRangeAsync(carrierServices);

        var shipmentAddresses = Sheet(workbook, "ShipmentAddresses")
            .Select(row => new ShipmentAddress
            {
                Id = ParseGuid(row, "Id"),
                AddressType = GetValue(row, "AddressType"),
                ContactName = GetValue(row, "ContactName"),
                CompanyName = ValueOrNull(row, "CompanyName"),
                Phone = ValueOrNull(row, "Phone"),
                Email = ValueOrNull(row, "Email"),
                AddressLine1 = GetValue(row, "AddressLine1"),
                AddressLine2 = ValueOrNull(row, "AddressLine2"),
                City = GetValue(row, "City"),
                StateOrProvince = GetValue(row, "StateOrProvince"),
                PostalCode = GetValue(row, "PostalCode"),
                Country = GetValue(row, "Country"),
                CreatedAtUtc = DateValue(row, "CreatedAtUtc", DateTime.UtcNow),
                CreatedBy = ValueOrDefault(row, "CreatedBy", SeedUser)
            })
            .ToList();
        await context.ShipmentAddresses.AddRangeAsync(shipmentAddresses);

        var deliveryRuns = Sheet(workbook, "DeliveryRuns")
            .Select(row => new DeliveryRun
            {
                Id = ParseGuid(row, "Id"),
                RunNumber = GetValue(row, "RunNumber"),
                Name = GetValue(row, "Name"),
                WarehouseId = ParseGuid(row, "WarehouseId"),
                PlannedStartUtc = DateValue(row, "PlannedStartUtc", DateTime.UtcNow),
                PlannedEndUtc = DateValue(row, "PlannedEndUtc", DateTime.UtcNow.AddHours(8)),
                ActualStartUtc = DateValueOrNull(row, "ActualStartUtc"),
                ActualEndUtc = DateValueOrNull(row, "ActualEndUtc"),
                DriverName = ValueOrNull(row, "DriverName"),
                VehicleNumber = ValueOrNull(row, "VehicleNumber"),
                RouteCode = ValueOrNull(row, "RouteCode"),
                Status = EnumValue(row, "Status", DeliveryRunStatus.Planned),
                Notes = ValueOrNull(row, "Notes"),
                CreatedAtUtc = DateValue(row, "CreatedAtUtc", DateTime.UtcNow),
                CreatedBy = ValueOrDefault(row, "CreatedBy", SeedUser)
            })
            .ToList();
        await context.DeliveryRuns.AddRangeAsync(deliveryRuns);

        var dockAppointments = Sheet(workbook, "DockAppointments")
            .Select(row => new DockAppointment
            {
                Id = ParseGuid(row, "Id"),
                AppointmentNumber = GetValue(row, "AppointmentNumber"),
                WarehouseId = ParseGuid(row, "WarehouseId"),
                CarrierId = GuidValueOrNull(row, "CarrierId"),
                DockCode = ValueOrNull(row, "DockCode"),
                TrailerNumber = ValueOrNull(row, "TrailerNumber"),
                DriverName = ValueOrNull(row, "DriverName"),
                ScheduledStartUtc = DateValue(row, "ScheduledStartUtc", DateTime.UtcNow),
                ScheduledEndUtc = DateValue(row, "ScheduledEndUtc", DateTime.UtcNow.AddHours(1)),
                ActualArrivalUtc = DateValueOrNull(row, "ActualArrivalUtc"),
                ActualDepartureUtc = DateValueOrNull(row, "ActualDepartureUtc"),
                Status = EnumValue(row, "Status", DockAppointmentStatus.Scheduled),
                Notes = ValueOrNull(row, "Notes"),
                CreatedAtUtc = DateValue(row, "CreatedAtUtc", DateTime.UtcNow),
                CreatedBy = ValueOrDefault(row, "CreatedBy", SeedUser)
            })
            .ToList();
        await context.DockAppointments.AddRangeAsync(dockAppointments);

        var shipments = Sheet(workbook, "Shipments")
            .Select(row => new Shipment
            {
                Id = ParseGuid(row, "Id"),
                ShipmentNumber = GetValue(row, "ShipmentNumber"),
                OrderId = GuidValueOrNull(row, "OrderId"),
                WarehouseId = ParseGuid(row, "WarehouseId"),
                CarrierId = GuidValueOrNull(row, "CarrierId"),
                CarrierServiceId = GuidValueOrNull(row, "CarrierServiceId"),
                OriginAddressId = ParseGuid(row, "OriginAddressId"),
                DestinationAddressId = ParseGuid(row, "DestinationAddressId"),
                DeliveryRunId = GuidValueOrNull(row, "DeliveryRunId"),
                DockAppointmentId = GuidValueOrNull(row, "DockAppointmentId"),
                Type = EnumValue(row, "Type", ShipmentType.Outbound),
                Status = EnumValue(row, "Status", ShipmentStatus.Draft),
                Priority = EnumValue(row, "Priority", ShipmentPriority.Normal),
                CustomerReference = ValueOrNull(row, "CustomerReference"),
                ExternalReference = ValueOrNull(row, "ExternalReference"),
                TrackingNumber = ValueOrNull(row, "TrackingNumber"),
                MasterTrackingNumber = ValueOrNull(row, "MasterTrackingNumber"),
                PlannedShipDateUtc = DateValueOrNull(row, "PlannedShipDateUtc"),
                PlannedDeliveryDateUtc = DateValueOrNull(row, "PlannedDeliveryDateUtc"),
                ActualShipDateUtc = DateValueOrNull(row, "ActualShipDateUtc"),
                ActualDeliveryDateUtc = DateValueOrNull(row, "ActualDeliveryDateUtc"),
                ScheduledPickupStartUtc = DateValueOrNull(row, "ScheduledPickupStartUtc"),
                ScheduledPickupEndUtc = DateValueOrNull(row, "ScheduledPickupEndUtc"),
                TotalWeight = DecimalValue(row, "TotalWeight"),
                TotalVolume = DecimalValue(row, "TotalVolume"),
                TotalPackages = IntValue(row, "TotalPackages"),
                FreightCost = DecimalValue(row, "FreightCost"),
                InsuranceCost = DecimalValue(row, "InsuranceCost"),
                OtherCharges = DecimalValue(row, "OtherCharges"),
                TotalShippingCost = DecimalValue(row, "TotalShippingCost"),
                CurrencyCode = ValueOrDefault(row, "CurrencyCode", "CAD"),
                ShippingTerms = ValueOrNull(row, "ShippingTerms"),
                Incoterm = ValueOrNull(row, "Incoterm"),
                IsPartialShipment = BoolValue(row, "IsPartialShipment", false),
                RequiresSignature = BoolValue(row, "RequiresSignature", false),
                IsFragile = BoolValue(row, "IsFragile", false),
                IsHazardous = BoolValue(row, "IsHazardous", false),
                IsTemperatureControlled = BoolValue(row, "IsTemperatureControlled", false),
                IsInsured = BoolValue(row, "IsInsured", false),
                IsCrossBorder = BoolValue(row, "IsCrossBorder", false),
                Notes = ValueOrNull(row, "Notes"),
                InternalNotes = ValueOrNull(row, "InternalNotes"),
                CreatedAtUtc = DateValue(row, "CreatedAtUtc", DateTime.UtcNow),
                CreatedBy = ValueOrDefault(row, "CreatedBy", SeedUser)
            })
            .ToList();
        await context.Shipments.AddRangeAsync(shipments);

        var shipmentItems = Sheet(workbook, "ShipmentItems")
            .Select(row => new ShipmentItem
            {
                Id = ParseGuid(row, "Id"),
                ShipmentId = ParseGuid(row, "ShipmentId"),
                OrderItemId = GuidValueOrNull(row, "OrderItemId"),
                ProductId = ParseGuid(row, "ProductId"),
                WarehouseId = ParseGuid(row, "WarehouseId"),
                UnitOfMeasureId = ParseGuid(row, "UnitOfMeasureId"),
                InventoryStockId = GuidValueOrNull(row, "InventoryStockId"),
                ProductionOrderId = GuidValueOrNull(row, "ProductionOrderId"),
                LineNumber = GetValue(row, "LineNumber"),
                OrderedQuantity = DecimalValue(row, "OrderedQuantity"),
                AllocatedQuantity = DecimalValue(row, "AllocatedQuantity"),
                PickedQuantity = DecimalValue(row, "PickedQuantity"),
                PackedQuantity = DecimalValue(row, "PackedQuantity"),
                ShippedQuantity = DecimalValue(row, "ShippedQuantity"),
                DeliveredQuantity = DecimalValue(row, "DeliveredQuantity"),
                ReturnedQuantity = DecimalValue(row, "ReturnedQuantity"),
                UnitWeight = DecimalValue(row, "UnitWeight"),
                UnitVolume = DecimalValue(row, "UnitVolume"),
                LotNumber = ValueOrNull(row, "LotNumber"),
                SerialNumber = ValueOrNull(row, "SerialNumber"),
                ExpiryDateUtc = DateValueOrNull(row, "ExpiryDateUtc"),
                Status = EnumValue(row, "Status", ShipmentItemStatus.Pending),
                Notes = ValueOrNull(row, "Notes"),
                CreatedAtUtc = DateValue(row, "CreatedAtUtc", DateTime.UtcNow),
                CreatedBy = ValueOrDefault(row, "CreatedBy", SeedUser)
            })
            .ToList();
        await context.ShipmentItems.AddRangeAsync(shipmentItems);

        var shipmentPackages = Sheet(workbook, "ShipmentPackages")
            .Select(row => new ShipmentPackage
            {
                Id = ParseGuid(row, "Id"),
                ShipmentId = ParseGuid(row, "ShipmentId"),
                PackageNumber = GetValue(row, "PackageNumber"),
                TrackingNumber = ValueOrNull(row, "TrackingNumber"),
                PackageType = GetValue(row, "PackageType"),
                Length = DecimalValue(row, "Length"),
                Width = DecimalValue(row, "Width"),
                Height = DecimalValue(row, "Height"),
                Weight = DecimalValue(row, "Weight"),
                DeclaredValue = DecimalValue(row, "DeclaredValue"),
                RequiresSpecialHandling = BoolValue(row, "RequiresSpecialHandling", false),
                IsFragile = BoolValue(row, "IsFragile", false),
                LabelUrl = ValueOrNull(row, "LabelUrl"),
                Barcode = ValueOrNull(row, "Barcode"),
                Status = EnumValue(row, "Status", PackageStatus.Draft),
                CreatedAtUtc = DateValue(row, "CreatedAtUtc", DateTime.UtcNow),
                CreatedBy = ValueOrDefault(row, "CreatedBy", SeedUser)
            })
            .ToList();
        await context.ShipmentPackages.AddRangeAsync(shipmentPackages);

        var packageItems = Sheet(workbook, "ShipmentPackageItems")
            .Select(row => new ShipmentPackageItem
            {
                Id = ParseGuid(row, "Id"),
                ShipmentPackageId = ParseGuid(row, "ShipmentPackageId"),
                ShipmentItemId = ParseGuid(row, "ShipmentItemId"),
                Quantity = DecimalValue(row, "Quantity"),
                CreatedAtUtc = DateValue(row, "CreatedAtUtc", DateTime.UtcNow),
                CreatedBy = ValueOrDefault(row, "CreatedBy", SeedUser)
            })
            .ToList();
        await context.ShipmentPackageItems.AddRangeAsync(packageItems);

        var trackingEvents = Sheet(workbook, "ShipmentTrackingEvents")
            .Select(row => new ShipmentTrackingEvent
            {
                Id = ParseGuid(row, "Id"),
                ShipmentId = ParseGuid(row, "ShipmentId"),
                EventCode = GetValue(row, "EventCode"),
                EventName = GetValue(row, "EventName"),
                Description = ValueOrNull(row, "Description"),
                EventTimeUtc = DateValue(row, "EventTimeUtc", DateTime.UtcNow),
                LocationName = ValueOrNull(row, "LocationName"),
                City = ValueOrNull(row, "City"),
                StateOrProvince = ValueOrNull(row, "StateOrProvince"),
                Country = ValueOrNull(row, "Country"),
                CarrierStatusCode = ValueOrNull(row, "CarrierStatusCode"),
                Source = ValueOrDefault(row, "Source", "Workbook"),
                IsCustomerVisible = BoolValue(row, "IsCustomerVisible", true),
                CreatedAtUtc = DateValue(row, "CreatedAtUtc", DateTime.UtcNow),
                CreatedBy = ValueOrDefault(row, "CreatedBy", SeedUser)
            })
            .ToList();
        await context.ShipmentTrackingEvents.AddRangeAsync(trackingEvents);

        var shipmentDocuments = Sheet(workbook, "ShipmentDocuments")
            .Select(row => new ShipmentDocument
            {
                Id = ParseGuid(row, "Id"),
                ShipmentId = ParseGuid(row, "ShipmentId"),
                DocumentType = EnumValue(row, "DocumentType", ShipmentDocumentType.Other),
                FileName = GetValue(row, "FileName"),
                FileUrl = GetValue(row, "FileUrl"),
                ContentType = ValueOrNull(row, "ContentType"),
                FileSizeBytes = LongValue(row, "FileSizeBytes"),
                IsCustomerVisible = BoolValue(row, "IsCustomerVisible", false),
                Notes = ValueOrNull(row, "Notes"),
                CreatedAtUtc = DateValue(row, "CreatedAtUtc", DateTime.UtcNow),
                CreatedBy = ValueOrDefault(row, "CreatedBy", SeedUser)
            })
            .ToList();
        await context.ShipmentDocuments.AddRangeAsync(shipmentDocuments);

        var statusHistories = Sheet(workbook, "ShipmentStatusHistories")
            .Select(row => new ShipmentStatusHistory
            {
                Id = ParseGuid(row, "Id"),
                ShipmentId = ParseGuid(row, "ShipmentId"),
                FromStatus = EnumValue(row, "FromStatus", ShipmentStatus.Draft),
                ToStatus = EnumValue(row, "ToStatus", ShipmentStatus.ReadyToDispatch),
                ChangedAtUtc = DateValue(row, "ChangedAtUtc", DateTime.UtcNow),
                ChangedBy = ValueOrDefault(row, "ChangedBy", SeedUser),
                Reason = ValueOrNull(row, "Reason"),
                CreatedAtUtc = DateValue(row, "CreatedAtUtc", DateTime.UtcNow),
                CreatedBy = ValueOrDefault(row, "CreatedBy", SeedUser)
            })
            .ToList();
        await context.ShipmentStatusHistories.AddRangeAsync(statusHistories);

        var shipmentExceptions = Sheet(workbook, "ShipmentExceptions")
            .Select(row => new ShipmentException
            {
                Id = ParseGuid(row, "Id"),
                ShipmentId = ParseGuid(row, "ShipmentId"),
                ExceptionType = EnumValue(row, "ExceptionType", ShipmentExceptionType.Other),
                Title = GetValue(row, "Title"),
                Description = ValueOrNull(row, "Description"),
                ReportedAtUtc = DateValue(row, "ReportedAtUtc", DateTime.UtcNow),
                ReportedBy = ValueOrNull(row, "ReportedBy"),
                IsResolved = BoolValue(row, "IsResolved", false),
                ResolvedAtUtc = DateValueOrNull(row, "ResolvedAtUtc"),
                ResolutionNote = ValueOrNull(row, "ResolutionNote"),
                CreatedAtUtc = DateValue(row, "CreatedAtUtc", DateTime.UtcNow),
                CreatedBy = ValueOrDefault(row, "CreatedBy", SeedUser)
            })
            .ToList();
        await context.ShipmentExceptions.AddRangeAsync(shipmentExceptions);

        var shipmentCharges = Sheet(workbook, "ShipmentCharges")
            .Select(row => new ShipmentCharge
            {
                Id = ParseGuid(row, "Id"),
                ShipmentId = ParseGuid(row, "ShipmentId"),
                ChargeType = EnumValue(row, "ChargeType", ShipmentChargeType.Other),
                Description = GetValue(row, "Description"),
                Amount = DecimalValue(row, "Amount"),
                CurrencyCode = ValueOrDefault(row, "CurrencyCode", "CAD"),
                CreatedAtUtc = DateValue(row, "CreatedAtUtc", DateTime.UtcNow),
                CreatedBy = ValueOrDefault(row, "CreatedBy", SeedUser)
            })
            .ToList();
        await context.ShipmentCharges.AddRangeAsync(shipmentCharges);

        var insurances = Sheet(workbook, "ShipmentInsurances")
            .Select(row => new ShipmentInsurance
            {
                Id = ParseGuid(row, "Id"),
                ShipmentId = ParseGuid(row, "ShipmentId"),
                ProviderName = GetValue(row, "ProviderName"),
                PolicyNumber = ValueOrNull(row, "PolicyNumber"),
                InsuredAmount = DecimalValue(row, "InsuredAmount"),
                PremiumAmount = DecimalValue(row, "PremiumAmount"),
                CurrencyCode = ValueOrDefault(row, "CurrencyCode", "CAD"),
                EffectiveDateUtc = DateValue(row, "EffectiveDateUtc", DateTime.UtcNow),
                ExpiryDateUtc = DateValueOrNull(row, "ExpiryDateUtc"),
                Status = EnumValue(row, "Status", InsuranceStatus.Pending),
                Notes = ValueOrNull(row, "Notes"),
                CreatedAtUtc = DateValue(row, "CreatedAtUtc", DateTime.UtcNow),
                CreatedBy = ValueOrDefault(row, "CreatedBy", SeedUser)
            })
            .ToList();
        await context.ShipmentInsurances.AddRangeAsync(insurances);

        var customsDocuments = Sheet(workbook, "CustomsDocuments")
            .Select(row => new CustomsDocument
            {
                Id = ParseGuid(row, "Id"),
                ShipmentId = ParseGuid(row, "ShipmentId"),
                DocumentType = EnumValue(row, "DocumentType", CustomsDocumentType.Other),
                DocumentNumber = GetValue(row, "DocumentNumber"),
                FileName = GetValue(row, "FileName"),
                FileUrl = GetValue(row, "FileUrl"),
                CountryOfOrigin = ValueOrNull(row, "CountryOfOrigin"),
                DestinationCountry = ValueOrNull(row, "DestinationCountry"),
                HarmonizedCode = ValueOrNull(row, "HarmonizedCode"),
                DeclaredCustomsValue = DecimalValueOrNull(row, "DeclaredCustomsValue"),
                CurrencyCode = ValueOrDefault(row, "CurrencyCode", "CAD"),
                IssuedAtUtc = DateValue(row, "IssuedAtUtc", DateTime.UtcNow),
                Notes = ValueOrNull(row, "Notes"),
                CreatedAtUtc = DateValue(row, "CreatedAtUtc", DateTime.UtcNow),
                CreatedBy = ValueOrDefault(row, "CreatedBy", SeedUser)
            })
            .ToList();
        await context.CustomsDocuments.AddRangeAsync(customsDocuments);

        var returnShipments = Sheet(workbook, "ReturnShipments")
            .Select(row => new ReturnShipment
            {
                Id = ParseGuid(row, "Id"),
                ReturnShipmentNumber = GetValue(row, "ReturnShipmentNumber"),
                ShipmentId = ParseGuid(row, "ShipmentId"),
                OrderId = GuidValueOrNull(row, "OrderId"),
                OriginAddressId = ParseGuid(row, "OriginAddressId"),
                DestinationAddressId = ParseGuid(row, "DestinationAddressId"),
                CarrierId = GuidValueOrNull(row, "CarrierId"),
                CarrierServiceId = GuidValueOrNull(row, "CarrierServiceId"),
                TrackingNumber = ValueOrNull(row, "TrackingNumber"),
                ReasonCode = GetValue(row, "ReasonCode"),
                ReasonDescription = ValueOrNull(row, "ReasonDescription"),
                Status = EnumValue(row, "Status", ReturnShipmentStatus.Requested),
                RequestedAtUtc = DateValue(row, "RequestedAtUtc", DateTime.UtcNow),
                ReceivedAtUtc = DateValueOrNull(row, "ReceivedAtUtc"),
                Notes = ValueOrNull(row, "Notes"),
                CreatedAtUtc = DateValue(row, "CreatedAtUtc", DateTime.UtcNow),
                CreatedBy = ValueOrDefault(row, "CreatedBy", SeedUser)
            })
            .ToList();
        await context.ReturnShipments.AddRangeAsync(returnShipments);

        var returnShipmentItems = Sheet(workbook, "ReturnShipmentItems")
            .Select(row => new ReturnShipmentItem
            {
                Id = ParseGuid(row, "Id"),
                ReturnShipmentId = ParseGuid(row, "ReturnShipmentId"),
                ShipmentItemId = ParseGuid(row, "ShipmentItemId"),
                ReturnedQuantity = DecimalValue(row, "ReturnedQuantity"),
                ReturnCondition = ValueOrNull(row, "ReturnCondition"),
                InspectionResult = ValueOrNull(row, "InspectionResult"),
                Notes = ValueOrNull(row, "Notes"),
                CreatedAtUtc = DateValue(row, "CreatedAtUtc", DateTime.UtcNow),
                CreatedBy = ValueOrDefault(row, "CreatedBy", SeedUser)
            })
            .ToList();
        await context.ReturnShipmentItems.AddRangeAsync(returnShipmentItems);

        await context.SaveChangesAsync();
        Console.WriteLine($"Imported shipment workbook: Shipments={shipments.Count}, Items={shipmentItems.Count}, Returns={returnShipments.Count}");
    }

    private static async Task PrintSummaryAsync(OperationIntelligenceDbContext context)
    {
        Console.WriteLine("Current row counts:");
        Console.WriteLine($"Users: {await context.Users.CountAsync()}");
        Console.WriteLine($"Roles: {await context.Roles.CountAsync()}");
        Console.WriteLine($"Inventory Products: {await context.Products.CountAsync()}");
        Console.WriteLine($"Orders: {await context.Orders.CountAsync()}");
        Console.WriteLine($"ProductionOrders: {await context.ProductionOrders.CountAsync()}");
        Console.WriteLine($"SchedulePlans: {await context.SchedulePlans.CountAsync()}");
        Console.WriteLine($"Shipments: {await context.Shipments.CountAsync()}");
    }

    private static ShipmentStatus MapShipmentStatus(string? statusText)
        => Enum.TryParse<ShipmentStatus>(statusText, true, out var status) ? status : ShipmentStatus.Draft;

    private static PaymentStatus DerivePaymentStatus(decimal paidAmount, decimal totalAmount)
    {
        if (paidAmount <= 0m) return PaymentStatus.Unpaid;
        if (paidAmount >= totalAmount && totalAmount > 0m) return PaymentStatus.Paid;
        return PaymentStatus.PartiallyPaid;
    }

    private static Dictionary<string, Guid> CreateLocalIdMap(IEnumerable<string> workbookIds)
        => workbookIds
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToDictionary(id => id, id => CreateStableGuid("local", id), StringComparer.OrdinalIgnoreCase);

    private static Guid? ResolveNormalizedWarehouseId(string? rawWarehouseId, IReadOnlyDictionary<string, Guid> warehouseMap)
    {
        if (string.IsNullOrWhiteSpace(rawWarehouseId))
        {
            return null;
        }

        if (warehouseMap.TryGetValue(rawWarehouseId, out var mappedWarehouseId))
        {
            return mappedWarehouseId;
        }

        return Guid.TryParse(rawWarehouseId, out var parsedWarehouseId) ? parsedWarehouseId : null;
    }

    private static NormalizedProductMap ResolveNormalizedProduct(
        string rawProductId,
        IReadOnlyDictionary<string, NormalizedProductMap> productMap)
    {
        if (productMap.TryGetValue(rawProductId, out var mappedProduct))
        {
            return mappedProduct;
        }

        if (Guid.TryParse(rawProductId, out var parsedProductId))
        {
            return new NormalizedProductMap(parsedProductId, null);
        }

        throw new InvalidOperationException($"Unable to normalize product id '{rawProductId}'.");
    }

    private static Guid? ResolveOptionalMappedGuid(string? workbookId, IReadOnlyDictionary<string, Guid> idMap)
        => string.IsNullOrWhiteSpace(workbookId) ? null : ResolveMappedGuid(workbookId, idMap);

    private static Guid CreateStableGuid(string scope, string value)
    {
        var bytes = SHA256.HashData(System.Text.Encoding.UTF8.GetBytes($"{scope}:{value}"));
        var guidBytes = new byte[16];
        Array.Copy(bytes, guidBytes, guidBytes.Length);
        return new Guid(guidBytes);
    }

    private static ScheduleJobStatus ResolveScheduleJobStatus(string? value)
    {
        if (string.Equals(value, "Planned", StringComparison.OrdinalIgnoreCase))
        {
            return ScheduleJobStatus.Scheduled;
        }

        return EnumValue(new Dictionary<string, string> { ["Value"] = value ?? string.Empty }, "Value", ScheduleJobStatus.Unscheduled);
    }

    private static DispatchStatus ResolveDispatchStatus(string? value)
    {
        if (string.Equals(value, "Queued", StringComparison.OrdinalIgnoreCase))
        {
            return DispatchStatus.NotDispatched;
        }

        return EnumValue(new Dictionary<string, string> { ["Value"] = value ?? string.Empty }, "Value", DispatchStatus.NotDispatched);
    }

    private static ResourceType ResolveResourceType(string? value)
    {
        if (string.Equals(value, "Operator", StringComparison.OrdinalIgnoreCase))
        {
            return ResourceType.Employee;
        }

        return EnumValue(new Dictionary<string, string> { ["Value"] = value ?? string.Empty }, "Value", ResourceType.WorkCenter);
    }

    private static Guid ResolveScheduleMaterialProductId(
        string? rawMaterialProductId,
        Guid productionOrderId,
        IReadOnlyDictionary<string, NormalizedProductMap> normalizedProductMap,
        IReadOnlyList<ProductionMaterialIssue> materialIssues,
        IReadOnlyList<Product> products)
    {
        if (!string.IsNullOrWhiteSpace(rawMaterialProductId))
        {
            if (normalizedProductMap.TryGetValue(rawMaterialProductId, out var normalizedProduct))
            {
                return normalizedProduct.ProductId;
            }

            if (Guid.TryParse(rawMaterialProductId, out var guidMaterialProductId) &&
                products.Any(product => product.Id == guidMaterialProductId))
            {
                return guidMaterialProductId;
            }
        }

        var issuesForOrder = materialIssues
            .Where(issue => issue.ProductionOrderId == productionOrderId)
            .OrderBy(issue => issue.CreatedAtUtc)
            .ToList();

        if (issuesForOrder.Count > 0)
        {
            if (int.TryParse(rawMaterialProductId, NumberStyles.Any, CultureInfo.InvariantCulture, out var ordinal))
            {
                return issuesForOrder[(Math.Abs(ordinal) - 1 + issuesForOrder.Count) % issuesForOrder.Count].MaterialProductId;
            }

            return issuesForOrder[0].MaterialProductId;
        }

        return products[0].Id;
    }

    private static Product ResolveProductForScheduleJob(
        Dictionary<string, string> row,
        Order? order,
        IReadOnlyDictionary<string, Product> productByNormalizedName,
        IReadOnlyList<Product> products)
    {
        if (order?.Items.FirstOrDefault() is { } orderItem)
        {
            var product = products.FirstOrDefault(candidate => candidate.Id == orderItem.ProductId);
            if (product != null) return product;
        }

        var productName = NormalizeLookup(ValueOrDefault(row, "ProductName", string.Empty));
        if (productByNormalizedName.TryGetValue(productName, out var exactMatch))
        {
            return exactMatch;
        }

        return products.FirstOrDefault(product => NormalizeLookup(product.Name).Contains(productName, StringComparison.Ordinal))
            ?? throw new InvalidOperationException($"Unable to resolve product for scheduling row with product '{ValueOrDefault(row, "ProductName", "unknown")}'.");
    }

    private static ProductionOrder? ResolveProductionOrder(Guid productId, Guid warehouseId, decimal plannedQuantity, IReadOnlyList<ProductionOrder> productionOrders)
    {
        return productionOrders
            .Where(order => order.ProductId == productId && order.WarehouseId == warehouseId)
            .OrderBy(order => Math.Abs(order.PlannedQuantity - plannedQuantity))
            .ThenBy(order => order.PlannedStartDate)
            .FirstOrDefault()
            ?? productionOrders
                .Where(order => order.ProductId == productId)
                .OrderBy(order => Math.Abs(order.PlannedQuantity - plannedQuantity))
                .ThenBy(order => order.PlannedStartDate)
                .FirstOrDefault()
            ?? productionOrders
                .Where(order => order.WarehouseId == warehouseId)
                .OrderBy(order => Math.Abs(order.PlannedQuantity - plannedQuantity))
                .ThenBy(order => order.PlannedStartDate)
                .FirstOrDefault()
            ?? productionOrders
                .OrderBy(order => Math.Abs(order.PlannedQuantity - plannedQuantity))
                .ThenBy(order => order.PlannedStartDate)
                .FirstOrDefault();
    }

    private static RoutingStep ResolveRoutingStep(
        Guid productId,
        Guid workCenterId,
        string operationName,
        int sequenceNo,
        IReadOnlyDictionary<Guid, List<Routing>> routingByProductId,
        IReadOnlyDictionary<Guid, List<RoutingStep>> routingStepsByRoutingId,
        IReadOnlyList<RoutingStep> allRoutingSteps)
    {
        var normalizedOperationName = NormalizeLookup(operationName);

        if (routingByProductId.TryGetValue(productId, out var routings))
        {
            foreach (var routing in routings.OrderByDescending(candidate => candidate.IsDefault).ThenBy(candidate => candidate.Version))
            {
                if (!routingStepsByRoutingId.TryGetValue(routing.Id, out var steps)) continue;

                var exactStep = steps.FirstOrDefault(step =>
                    step.WorkCenterId == workCenterId &&
                    NormalizeLookup(step.OperationName) == normalizedOperationName);
                if (exactStep != null) return exactStep;

                var sequenceStep = steps.FirstOrDefault(step =>
                    step.WorkCenterId == workCenterId &&
                    step.Sequence == sequenceNo);
                if (sequenceStep != null) return sequenceStep;
            }
        }

        return allRoutingSteps.FirstOrDefault(step =>
                   step.WorkCenterId == workCenterId &&
                   NormalizeLookup(step.OperationName) == normalizedOperationName)
               ?? allRoutingSteps.First(step => step.WorkCenterId == workCenterId);
    }

    private static ProductionExecution? ResolveExecution(
        Guid productionOrderId,
        Guid workCenterId,
        Guid? machineId,
        IReadOnlyDictionary<Guid, List<ProductionExecution>> executionsByProductionOrderId)
    {
        if (!executionsByProductionOrderId.TryGetValue(productionOrderId, out var executions))
        {
            return null;
        }

        return executions.FirstOrDefault(execution => execution.WorkCenterId == workCenterId && execution.MachineId == machineId)
               ?? executions.FirstOrDefault(execution => execution.WorkCenterId == workCenterId)
               ?? executions.FirstOrDefault();
    }

    private static Shift? ResolveShiftForDate(Guid workCenterId, DateTime momentUtc, IReadOnlyDictionary<Guid, List<Shift>> shiftsByWorkCenter)
    {
        if (!shiftsByWorkCenter.TryGetValue(workCenterId, out var shifts))
        {
            return null;
        }

        var time = momentUtc.TimeOfDay;
        return shifts.FirstOrDefault(shift => TimeFallsWithinShift(time, shift.StartTime, shift.EndTime, shift.CrossesMidnight))
               ?? shifts.FirstOrDefault();
    }

    private static bool TimeFallsWithinShift(TimeSpan time, TimeSpan start, TimeSpan end, bool crossesMidnight)
    {
        if (!crossesMidnight)
        {
            return time >= start && time <= end;
        }

        return time >= start || time <= end;
    }

    private static DispatchStatus MapDispatchStatus(ScheduleOperationStatus status) => status switch
    {
        ScheduleOperationStatus.Completed => DispatchStatus.Completed,
        ScheduleOperationStatus.Running => DispatchStatus.InExecution,
        ScheduleOperationStatus.Released => DispatchStatus.Dispatched,
        ScheduleOperationStatus.Scheduled => DispatchStatus.Acknowledged,
        _ => DispatchStatus.NotDispatched
    };

    private static int CalculatePriorityScore(SchedulePriority priority, int sequenceNo) => priority switch
    {
        SchedulePriority.Critical => 1000 - sequenceNo,
        SchedulePriority.Urgent => 900 - sequenceNo,
        SchedulePriority.High => 800 - sequenceNo,
        SchedulePriority.Normal => 700 - sequenceNo,
        _ => 600 - sequenceNo
    };

    private static Guid ResolveScheduleAuditEntityId(
        string entityName,
        IReadOnlyList<SchedulePlan> plans,
        IReadOnlyList<ScheduleJob> jobs,
        IReadOnlyList<ScheduleOperation> operations)
    {
        var normalized = NormalizeLookup(entityName);
        if (normalized.Contains("plan", StringComparison.Ordinal))
        {
            return plans.First().Id;
        }

        if (normalized.Contains("job", StringComparison.Ordinal))
        {
            return jobs.First().Id;
        }

        return operations.First().Id;
    }

    private static string BuildOperationCode(string operationName, int sequence)
    {
        var cleaned = new string(operationName.Where(char.IsLetterOrDigit).ToArray());
        if (string.IsNullOrWhiteSpace(cleaned))
        {
            cleaned = "OP";
        }

        return $"{cleaned.ToUpperInvariant()}-{sequence:000}";
    }

    private static string BuildShiftCode(string shiftName)
    {
        var cleaned = new string(shiftName.Where(char.IsLetterOrDigit).ToArray()).ToUpperInvariant();
        return string.IsNullOrWhiteSpace(cleaned) ? "SHIFT" : cleaned;
    }

    private static int CalculateShiftMinutes(TimeSpan start, TimeSpan end, bool crossesMidnight)
    {
        if (!crossesMidnight)
        {
            return (int)(end - start).TotalMinutes;
        }

        return (int)((TimeSpan.FromHours(24) - start) + end).TotalMinutes;
    }

    private static string BuildUniqueUserName(string email, string firstName, string lastName, ISet<string> usedNames)
    {
        var baseName = ValueFromEmail(email);
        if (string.IsNullOrWhiteSpace(baseName))
        {
            baseName = $"{firstName}.{lastName}";
        }

        baseName = new string(baseName.Where(ch => char.IsLetterOrDigit(ch) || ch is '.' or '_' or '-').ToArray());
        if (string.IsNullOrWhiteSpace(baseName))
        {
            baseName = "user";
        }

        var candidate = baseName;
        var index = 1;
        while (!usedNames.Add(candidate))
        {
            candidate = $"{baseName}{index++}";
        }

        return candidate;
    }

    private static string ValueFromEmail(string email)
    {
        var atIndex = email.IndexOf('@');
        return atIndex > 0 ? email[..atIndex] : email;
    }

    private static string NormalizeUpper(string value) => value.Trim().ToUpperInvariant();

    private static Guid ResolveMappedGuid(string workbookId, IReadOnlyDictionary<string, Guid> idMap)
    {
        if (idMap.TryGetValue(workbookId, out var mappedId))
        {
            return mappedId;
        }

        if (Guid.TryParse(workbookId, out var parsedGuid))
        {
            return parsedGuid;
        }

        throw new InvalidOperationException($"Unable to resolve mapped Guid for identifier '{workbookId}'.");
    }

    private static Dictionary<string, Guid> BuildExternalGuidMap(IEnumerable<string> externalIds, IEnumerable<Guid> targetIds)
    {
        var normalizedExternalIds = externalIds
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(id => id, StringComparer.OrdinalIgnoreCase)
            .ToList();
        var orderedTargetIds = targetIds.Distinct().OrderBy(id => id).ToList();

        if (orderedTargetIds.Count == 0)
        {
            throw new InvalidOperationException("Cannot build an external id map without target ids.");
        }

        var map = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);
        for (var index = 0; index < normalizedExternalIds.Count; index++)
        {
            var externalId = normalizedExternalIds[index];
            map[externalId] = Guid.TryParse(externalId, out var parsedGuid) && orderedTargetIds.Contains(parsedGuid)
                ? parsedGuid
                : orderedTargetIds[index % orderedTargetIds.Count];
        }

        return map;
    }

    private static string NormalizeLookup(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return string.Empty;

        return new string(value
            .Trim()
            .ToUpperInvariant()
            .Where(char.IsLetterOrDigit)
            .ToArray());
    }

    private static string HashToken(string rawToken)
    {
        var bytes = SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(rawToken));
        return Convert.ToBase64String(bytes);
    }

    private static string? LoadConnectionStringFromApiAppSettings()
    {
        var path = Path.Combine("OperationIntelligence.Api", "appsettings.json");
        if (!File.Exists(path))
        {
            return null;
        }

        using var stream = File.OpenRead(path);
        using var document = JsonDocument.Parse(stream);

        return document.RootElement.TryGetProperty("ConnectionStrings", out var connectionStrings) &&
               connectionStrings.TryGetProperty("DefaultConnection", out var value)
            ? value.GetString()
            : null;
    }

    private static bool HasFlag(IEnumerable<string> args, string flag)
        => args.Any(arg => string.Equals(arg, flag, StringComparison.OrdinalIgnoreCase));

    private static string? GetArg(IReadOnlyList<string> args, string name)
    {
        for (var i = 0; i < args.Count; i++)
        {
            if (!string.Equals(args[i], name, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            return i + 1 < args.Count ? args[i + 1] : null;
        }

        return null;
    }

    private static void EnsureWorkbookExists(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Workbook not found: {path}", path);
        }
    }

    private static List<Dictionary<string, string>> Sheet(
        IReadOnlyDictionary<string, List<Dictionary<string, string>>> workbook,
        string sheetName)
        => workbook.TryGetValue(sheetName, out var rows) ? rows : [];

    private static Dictionary<string, List<Dictionary<string, string>>> ReadWorkbookSheets(string workbookPath)
    {
        var sheets = new Dictionary<string, List<Dictionary<string, string>>>(StringComparer.OrdinalIgnoreCase);

        using var zip = ZipFile.OpenRead(workbookPath);
        var mainNs = XNamespace.Get("http://schemas.openxmlformats.org/spreadsheetml/2006/main");
        var documentRelNs = XNamespace.Get("http://schemas.openxmlformats.org/officeDocument/2006/relationships");
        var packageRelNs = XNamespace.Get("http://schemas.openxmlformats.org/package/2006/relationships");

        var sharedStrings = ReadSharedStrings(zip, mainNs);
        var workbookDoc = XDocument.Load(zip.GetEntry("xl/workbook.xml")!.Open());
        var workbookRels = XDocument.Load(zip.GetEntry("xl/_rels/workbook.xml.rels")!.Open());
        var relMap = workbookRels.Descendants(packageRelNs + "Relationship")
            .Where(rel => rel.Attribute("Id") != null && rel.Attribute("Target") != null)
            .ToDictionary(
                rel => rel.Attribute("Id")!.Value,
                rel => rel.Attribute("Target")!.Value,
                StringComparer.OrdinalIgnoreCase);

        foreach (var sheetElement in workbookDoc.Descendants(mainNs + "sheet"))
        {
            var sheetName = sheetElement.Attribute("name")?.Value;
            var relId = sheetElement.Attribute(documentRelNs + "id")?.Value;
            if (string.IsNullOrWhiteSpace(sheetName) || string.IsNullOrWhiteSpace(relId) || !relMap.TryGetValue(relId, out var target))
            {
                continue;
            }

            var entryPath = NormalizeZipPath(Path.Combine("xl", target));
            var entry = zip.GetEntry(entryPath);
            if (entry == null)
            {
                continue;
            }

            var rows = ReadSheetRows(entry.Open(), mainNs, sharedStrings);
            sheets[sheetName] = rows;
        }

        return sheets;
    }

    private static List<string> ReadSharedStrings(ZipArchive zip, XNamespace mainNs)
    {
        var entry = zip.GetEntry("xl/sharedStrings.xml");
        if (entry == null)
        {
            return [];
        }

        var document = XDocument.Load(entry.Open());
        return document.Descendants(mainNs + "si")
            .Select(node => string.Concat(node.Descendants(mainNs + "t").Select(text => text.Value)))
            .ToList();
    }

    private static List<Dictionary<string, string>> ReadSheetRows(Stream stream, XNamespace mainNs, IReadOnlyList<string> sharedStrings)
    {
        var document = XDocument.Load(stream);
        var rowElements = document.Descendants(mainNs + "row").ToList();
        if (rowElements.Count == 0)
        {
            return [];
        }

        var headerMap = ReadRowCells(rowElements[0], mainNs, sharedStrings)
            .Where(pair => !string.IsNullOrWhiteSpace(pair.Value))
            .ToDictionary(pair => pair.Key, pair => pair.Value, EqualityComparer<int>.Default);

        var rows = new List<Dictionary<string, string>>();
        foreach (var rowElement in rowElements.Skip(1))
        {
            var cells = ReadRowCells(rowElement, mainNs, sharedStrings);
            var row = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var header in headerMap)
            {
                row[header.Value] = cells.TryGetValue(header.Key, out var value) ? value : string.Empty;
            }

            rows.Add(row);
        }

        return rows;
    }

    private static Dictionary<int, string> ReadRowCells(XElement rowElement, XNamespace mainNs, IReadOnlyList<string> sharedStrings)
    {
        var values = new Dictionary<int, string>();
        foreach (var cell in rowElement.Elements(mainNs + "c"))
        {
            var cellRef = cell.Attribute("r")?.Value;
            var columnIndex = GetColumnIndex(cellRef);
            values[columnIndex] = ReadCellValue(cell, mainNs, sharedStrings);
        }

        return values;
    }

    private static string ReadCellValue(XElement cell, XNamespace mainNs, IReadOnlyList<string> sharedStrings)
    {
        var cellType = cell.Attribute("t")?.Value;
        if (string.Equals(cellType, "s", StringComparison.OrdinalIgnoreCase))
        {
            var sharedIndexValue = cell.Element(mainNs + "v")?.Value;
            return int.TryParse(sharedIndexValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var sharedIndex) &&
                   sharedIndex >= 0 && sharedIndex < sharedStrings.Count
                ? sharedStrings[sharedIndex]
                : string.Empty;
        }

        if (string.Equals(cellType, "inlineStr", StringComparison.OrdinalIgnoreCase))
        {
            return string.Concat(cell.Descendants(mainNs + "t").Select(text => text.Value)).Trim();
        }

        return cell.Element(mainNs + "v")?.Value?.Trim() ?? string.Empty;
    }

    private static int GetColumnIndex(string? cellReference)
    {
        if (string.IsNullOrWhiteSpace(cellReference))
        {
            return 0;
        }

        var index = 0;
        foreach (var ch in cellReference)
        {
            if (!char.IsLetter(ch))
            {
                break;
            }

            index = (index * 26) + (char.ToUpperInvariant(ch) - 'A' + 1);
        }

        return Math.Max(0, index - 1);
    }

    private static string NormalizeZipPath(string path)
        => path.Replace('\\', '/')
            .Split('/', StringSplitOptions.RemoveEmptyEntries)
            .Aggregate(new Stack<string>(), (stack, segment) =>
            {
                if (segment == ".")
                {
                    return stack;
                }

                if (segment == "..")
                {
                    if (stack.Count > 0)
                    {
                        stack.Pop();
                    }

                    return stack;
                }

                stack.Push(segment);
                return stack;
            })
            .Reverse()
            .Aggregate(string.Empty, (current, segment) => string.IsNullOrEmpty(current) ? segment : $"{current}/{segment}");

    private static string GetValue(IReadOnlyDictionary<string, string> row, string column)
        => ValueOrNull(row, column) ?? throw new InvalidOperationException($"Missing required value for column '{column}'.");

    private static string? ValueOrNull(IReadOnlyDictionary<string, string> row, string column)
        => row.TryGetValue(column, out var value) && !string.IsNullOrWhiteSpace(value) ? value.Trim() : null;

    private static string ValueOrDefault(IReadOnlyDictionary<string, string> row, string column, string? fallback)
        => ValueOrNull(row, column) ?? (fallback ?? string.Empty);

    private static Guid ParseGuid(IReadOnlyDictionary<string, string> row, string column)
    {
        var value = GetValue(row, column);
        return Guid.TryParse(value, out var guid)
            ? guid
            : throw new InvalidOperationException($"Invalid GUID in column '{column}': {value}");
    }

    private static Guid? GuidValueOrNull(IReadOnlyDictionary<string, string> row, string column)
    {
        var value = ValueOrNull(row, column);
        return Guid.TryParse(value, out var guid) ? guid : null;
    }

    private static bool BoolValue(IReadOnlyDictionary<string, string> row, string column, bool fallback)
    {
        var value = ValueOrNull(row, column);
        if (value == null)
        {
            return fallback;
        }

        if (string.Equals(value, "1", StringComparison.OrdinalIgnoreCase)) return true;
        if (string.Equals(value, "0", StringComparison.OrdinalIgnoreCase)) return false;
        return bool.TryParse(value, out var parsed) ? parsed : fallback;
    }

    private static int IntValue(IReadOnlyDictionary<string, string> row, string column)
    {
        var value = ValueOrNull(row, column);
        if (value == null) return 0;
        if (int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsed)) return parsed;
        return decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var decimalValue)
            ? (int)decimalValue
            : 0;
    }

    private static long LongValue(IReadOnlyDictionary<string, string> row, string column)
    {
        var value = ValueOrNull(row, column);
        if (value == null) return 0L;
        if (long.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsed)) return parsed;
        return decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var decimalValue)
            ? (long)decimalValue
            : 0L;
    }

    private static decimal DecimalValue(IReadOnlyDictionary<string, string> row, string column)
    {
        var value = ValueOrNull(row, column);
        return decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsed) ? parsed : 0m;
    }

    private static decimal? DecimalValueOrNull(IReadOnlyDictionary<string, string> row, string column)
    {
        var value = ValueOrNull(row, column);
        return decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsed) ? parsed : null;
    }

    private static DateTime DateValue(IReadOnlyDictionary<string, string> row, string column, DateTime fallback)
        => DateValueOrNull(row, column) ?? fallback;

    private static DateTime? DateValueOrNull(IReadOnlyDictionary<string, string> row, string column)
    {
        var value = ValueOrNull(row, column);
        if (value == null)
        {
            return null;
        }

        if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var oaDate))
        {
            return DateTime.SpecifyKind(DateTime.FromOADate(oaDate), DateTimeKind.Utc);
        }

        if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var parsed))
        {
            return parsed;
        }

        return null;
    }

    private static TimeSpan TimeValue(IReadOnlyDictionary<string, string> row, string column)
    {
        var value = ValueOrNull(row, column);
        if (value == null)
        {
            return TimeSpan.Zero;
        }

        if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var oaTime))
        {
            return DateTime.FromOADate(oaTime).TimeOfDay;
        }

        if (TimeSpan.TryParse(value, CultureInfo.InvariantCulture, out var parsedTime))
        {
            return parsedTime;
        }

        if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDateTime))
        {
            return parsedDateTime.TimeOfDay;
        }

        return TimeSpan.Zero;
    }

    private static TEnum EnumValue<TEnum>(IReadOnlyDictionary<string, string> row, string column, TEnum fallback)
        where TEnum : struct, Enum
    {
        var value = ValueOrNull(row, column);
        if (value == null)
        {
            return fallback;
        }

        if (Enum.TryParse<TEnum>(value, true, out var parsed))
        {
            return parsed;
        }

        if (int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var enumNumber) &&
            Enum.IsDefined(typeof(TEnum), enumNumber))
        {
            return (TEnum)Enum.ToObject(typeof(TEnum), enumNumber);
        }

        var normalized = NormalizeLookup(value);
        foreach (var name in Enum.GetNames<TEnum>())
        {
            if (NormalizeLookup(name) == normalized)
            {
                return Enum.Parse<TEnum>(name);
            }
        }

        return fallback;
    }
}
