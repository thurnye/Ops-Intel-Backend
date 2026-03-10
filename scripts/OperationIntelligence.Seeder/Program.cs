using System.Globalization;
using System.IO.Compression;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using OperationIntelligence.DB;
using OperationIntelligence.DB.Entities;

var now = DateTime.UtcNow;
var random = new Random(260309);

var targetCount = ParseIntArg(args, "--count", 60);
targetCount = Math.Clamp(targetCount, 50, 200);
var summaryOnly = args.Any(x => string.Equals(x, "--summary-only", StringComparison.OrdinalIgnoreCase));
var resetDatabase = args.Any(x => string.Equals(x, "--reset", StringComparison.OrdinalIgnoreCase));

var connectionString = GetArg(args, "--connection") ?? LoadConnectionStringFromApiAppSettings();
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Connection string was not provided and could not be loaded from OperationIntelligence.Api/appsettings.json");
}

Console.WriteLine($"Starting seeding with target count {targetCount} per entity group...");

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

var runId = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
var seedUser = "seed-script";

await SeedInventoryBaselineIfMissingAsync(context, random, runId, seedUser);

var products = await context.Products.AsNoTracking().Where(x => !x.IsDeleted).ToListAsync();
var warehouses = await context.Warehouses.AsNoTracking().Where(x => !x.IsDeleted).ToListAsync();
var uoms = await context.UnitsOfMeasure.AsNoTracking().Where(x => !x.IsDeleted).ToListAsync();
var stocks = await context.InventoryStocks.AsNoTracking().Where(x => !x.IsDeleted).ToListAsync();

if (products.Count < 10 || warehouses.Count < 2 || uoms.Count < 1)
{
    throw new InvalidOperationException(
        $"Inventory baseline is too small. Products={products.Count}, Warehouses={warehouses.Count}, UOMs={uoms.Count}. Seed inventory first.");
}

var users = await SeedUsersAsync(context, random, runId, seedUser, targetCount);
var orders = await SeedOrdersAsync(context, random, runId, seedUser, targetCount, products, warehouses, uoms);

var production = await SeedProductionAsync(
    context,
    random,
    runId,
    seedUser,
    targetCount,
    products,
    warehouses,
    uoms,
    users,
    orders.OrderItems);

var scheduling = await SeedSchedulingAsync(
    context,
    random,
    runId,
    seedUser,
    targetCount,
    products,
    warehouses,
    production,
    orders.Orders,
    orders.OrderItems);

var shipment = await SeedShipmentsAsync(
    context,
    random,
    runId,
    seedUser,
    targetCount,
    products,
    warehouses,
    uoms,
    stocks,
    orders.Orders,
    orders.OrderItems,
    production.ProductionOrders);

Console.WriteLine("\nSeeding complete.");
Console.WriteLine($"Users: {users.Count}");
Console.WriteLine($"Orders: {orders.Orders.Count}, Items: {orders.OrderItems.Count}, Addresses: {orders.OrderAddresses.Count}");
Console.WriteLine($"ProductionOrders: {production.ProductionOrders.Count}, Executions: {production.ProductionExecutions.Count}");
Console.WriteLine($"SchedulePlans: {scheduling.SchedulePlans.Count}, Jobs: {scheduling.ScheduleJobs.Count}, Ops: {scheduling.ScheduleOperations.Count}");
Console.WriteLine($"Shipments: {shipment.Shipments.Count}, Items: {shipment.ShipmentItems.Count}, ReturnShipments: {shipment.ReturnShipments.Count}");

static string? LoadConnectionStringFromApiAppSettings()
{
    var path = Path.Combine("OperationIntelligence.Api", "appsettings.json");
    if (!File.Exists(path)) return null;

    using var stream = File.OpenRead(path);
    using var doc = JsonDocument.Parse(stream);

    if (!doc.RootElement.TryGetProperty("ConnectionStrings", out var csNode)) return null;
    if (!csNode.TryGetProperty("DefaultConnection", out var defaultConnection)) return null;

    return defaultConnection.GetString();
}

static string? GetArg(string[] args, string name)
{
    for (var i = 0; i < args.Length; i++)
    {
        if (!string.Equals(args[i], name, StringComparison.OrdinalIgnoreCase)) continue;
        if (i + 1 < args.Length) return args[i + 1];
    }

    return null;
}

static int ParseIntArg(string[] args, string name, int defaultValue)
{
    var value = GetArg(args, name);
    return int.TryParse(value, out var parsed) ? parsed : defaultValue;
}

static T Pick<T>(Random random, IReadOnlyList<T> values)
{
    return values[random.Next(values.Count)];
}

static decimal NextDecimal(Random random, decimal min, decimal max)
{
    var value = random.NextDouble();
    return Math.Round((decimal)value * (max - min) + min, 2);
}

static DateTime NextDate(Random random, DateTime fromUtc, DateTime toUtc)
{
    if (toUtc <= fromUtc) return fromUtc;
    var range = toUtc - fromUtc;
    var offset = TimeSpan.FromTicks((long)(random.NextDouble() * range.Ticks));
    return fromUtc.Add(offset);
}

static TEnum RandomEnum<TEnum>(Random random) where TEnum : struct, Enum
{
    var values = Enum.GetValues<TEnum>();
    return values[random.Next(values.Length)];
}

static List<string> GetStreetNames() =>
[
    "King St", "Queen St", "Front St", "Bay St", "Yonge St", "Wellington St",
    "Granville St", "Robson St", "Jasper Ave", "Portage Ave", "Bloor St", "Main St"
];

static List<(string City, string Province)> GetCityDirectory() =>
[
    ("Toronto", "ON"),
    ("Mississauga", "ON"),
    ("Vaughan", "ON"),
    ("Montreal", "QC"),
    ("Laval", "QC"),
    ("Quebec City", "QC"),
    ("Calgary", "AB"),
    ("Edmonton", "AB"),
    ("Vancouver", "BC"),
    ("Surrey", "BC"),
    ("Winnipeg", "MB"),
    ("Halifax", "NS")
];

static List<string> GetPersonDirectory() =>
[
    "Liam Carter", "Noah Patel", "Oliver Nguyen", "Elijah Thompson", "Mateo Chen",
    "Lucas Martin", "Mason Wright", "Ethan Singh", "James Brown", "Benjamin Wilson",
    "Ava Johnson", "Emma Clark", "Sophia Lewis", "Isabella Hall", "Mia Young",
    "Charlotte Allen", "Amelia King", "Harper Scott", "Evelyn Green", "Abigail Baker",
    "Logan Murphy", "Jacob Lee", "Michael Adams", "Daniel Cooper", "Henry Bell",
    "Ella Brooks", "Scarlett Morris", "Grace Campbell", "Chloe Evans", "Nora Mitchell"
];

static List<string> GetCustomerCompanies() =>
[
    "NorthStar Retail Group", "MapleLine Stores", "Prairie Home Goods", "Summit Tech Solutions",
    "Lakeside Industrial", "Aurora Health Supply", "Vertex Facilities Inc", "Redwood Distribution",
    "Skyline Commercial", "Beacon Manufacturing", "BlueHarbor Wholesale", "Urban Transit Services"
];

static List<(string Name, string Code, string Description)> GetProductFamilies() =>
[
    ("Thermal Label Printer", "TLP", "industrial barcode and shipping label printing"),
    ("Handheld Scanner", "HHS", "inventory counting and receiving operations"),
    ("Safety Helmet", "SHM", "site safety compliance and protection"),
    ("Nitrile Gloves", "NGL", "general handling and protection tasks"),
    ("Power Drill", "PDR", "assembly and maintenance operations"),
    ("Industrial Fan", "IFN", "warehouse ventilation and cooling"),
    ("Ethernet Switch", "ETH", "network edge and IoT connectivity"),
    ("Pallet Jack", "PLJ", "material movement inside facilities"),
    ("Storage Bin", "SBN", "parts picking and shelf organization"),
    ("Barcode Labels", "LBL", "packaging and outbound shipment tagging"),
    ("Tool Set", "TLS", "field and line maintenance"),
    ("LED Work Light", "LGT", "production and staging illumination")
];

static string BuildCanadianPostalCode(Random random)
{
    const string letters = "ABCEGHJKLMNPRSTVXY";
    return $"{letters[random.Next(letters.Length)]}{random.Next(0, 10)}{letters[random.Next(letters.Length)]} {random.Next(0, 10)}{letters[random.Next(letters.Length)]}{random.Next(0, 10)}";
}

static async Task PrintSummaryAsync(OperationIntelligenceDbContext context)
{
    Console.WriteLine("Current row counts:");
    Console.WriteLine($"Inventory Products: {await context.Products.CountAsync()}");
    Console.WriteLine($"Inventory Warehouses: {await context.Warehouses.CountAsync()}");
    Console.WriteLine($"Inventory Stocks: {await context.InventoryStocks.CountAsync()}");
    Console.WriteLine($"Orders: {await context.Orders.CountAsync()}");
    Console.WriteLine($"OrderItems: {await context.OrderItems.CountAsync()}");
    Console.WriteLine($"ProductionOrders: {await context.ProductionOrders.CountAsync()}");
    Console.WriteLine($"ProductionExecutions: {await context.ProductionExecutions.CountAsync()}");
    Console.WriteLine($"SchedulePlans: {await context.SchedulePlans.CountAsync()}");
    Console.WriteLine($"ScheduleJobs: {await context.ScheduleJobs.CountAsync()}");
    Console.WriteLine($"ScheduleOperations: {await context.ScheduleOperations.CountAsync()}");
    Console.WriteLine($"Shipments: {await context.Shipments.CountAsync()}");
    Console.WriteLine($"ShipmentItems: {await context.ShipmentItems.CountAsync()}");
    Console.WriteLine($"ReturnShipments: {await context.ReturnShipments.CountAsync()}");
}

static async Task SeedInventoryBaselineIfMissingAsync(
    OperationIntelligenceDbContext context,
    Random random,
    string runId,
    string seedUser)
{
    var hasProducts = await context.Products.AsNoTracking().AnyAsync();
    var hasWarehouses = await context.Warehouses.AsNoTracking().AnyAsync();
    var hasUoms = await context.UnitsOfMeasure.AsNoTracking().AnyAsync();
    if (hasProducts && hasWarehouses && hasUoms)
    {
        return;
    }

    var inventoryWorkbookPath = Path.Combine("scripts", "inventory_seed_data_relational.xlsx");
    if (File.Exists(inventoryWorkbookPath))
    {
        Console.WriteLine("Inventory workbook found. Importing inventory data from scripts/inventory_seed_data_relational.xlsx ...");
        await ImportInventoryFromWorkbookAsync(context, inventoryWorkbookPath, seedUser);
        return;
    }

    Console.WriteLine("Inventory baseline not found. Seeding inventory entities first...");

    var uoms = new List<UnitOfMeasure>
    {
        new() { Name = "Each", Symbol = "ea", CreatedBy = seedUser },
        new() { Name = "Box", Symbol = "box", CreatedBy = seedUser },
        new() { Name = "Case", Symbol = "cs", CreatedBy = seedUser },
        new() { Name = "Pallet", Symbol = "plt", CreatedBy = seedUser },
        new() { Name = "Kilogram", Symbol = "kg", CreatedBy = seedUser },
        new() { Name = "Gram", Symbol = "g", CreatedBy = seedUser },
        new() { Name = "Liter", Symbol = "l", CreatedBy = seedUser },
        new() { Name = "Meter", Symbol = "m", CreatedBy = seedUser }
    };
    await context.UnitsOfMeasure.AddRangeAsync(uoms);

    var categoryNames = new[]
    {
        "Consumer Electronics", "Office Supplies", "Packaging Materials", "Safety Equipment", "Maintenance Parts",
        "Industrial Components", "Furniture", "Cleaning Supplies", "IT Accessories", "Tools",
        "Appliances", "Lighting", "Storage Solutions", "Food Service", "Medical Consumables",
        "Automotive Parts", "Textiles", "HVAC Supplies", "Electrical", "Plumbing"
    };
    var categories = categoryNames.Select(x => new Category
    {
        Name = x,
        Description = $"{x} catalog",
        CreatedBy = seedUser
    }).ToList();
    await context.Categories.AddRangeAsync(categories);

    var brandNames = new[]
    {
        "3M", "Bosch", "Honeywell", "Stanley", "Milwaukee",
        "Logitech", "Philips", "Dell", "HP", "Lenovo",
        "Brother", "Canon", "Epson", "Dyson", "Panasonic",
        "Samsung", "LG", "Makita", "Whirlpool", "Siemens"
    };
    var brands = brandNames.Select(x => new Brand
    {
        Name = x,
        Description = $"{x} product line",
        CreatedBy = seedUser
    }).ToList();
    await context.Brands.AddRangeAsync(brands);

    var warehousePrefixes = new[]
    {
        "Toronto Central DC", "Toronto East Fulfillment", "Mississauga West Hub", "Vaughan North DC",
        "Montreal Main Warehouse", "Laval Metro Hub", "Ottawa Capital DC", "Quebec City Distribution",
        "Calgary Plains DC", "Edmonton North Warehouse", "Vancouver Port Hub", "Surrey Fulfillment",
        "Winnipeg Prairie Warehouse", "Halifax Atlantic Hub", "Regina Inland DC", "Saskatoon Supply Hub",
        "Hamilton Regional DC", "Kitchener Tech Fulfillment", "Burnaby Urban Warehouse", "Victoria Island Hub"
    };
    var cityDirectory = GetCityDirectory();
    var warehouses = Enumerable.Range(1, 20).Select(i =>
    {
        var cityInfo = cityDirectory[(i - 1) % cityDirectory.Count];
        return new Warehouse
        {
            Name = warehousePrefixes[i - 1],
            Code = $"WH-{cityInfo.City[..Math.Min(3, cityInfo.City.Length)].ToUpperInvariant()}-{i:00}",
            Description = $"{cityInfo.City} regional warehouse",
            AddressLine1 = $"{150 + i} {Pick(random, GetStreetNames())}",
            City = cityInfo.City,
            StateOrProvince = cityInfo.Province,
            PostalCode = BuildCanadianPostalCode(random),
            Country = "Canada",
            IsActive = true,
            CreatedBy = seedUser
        };
    }).ToList();
    await context.Warehouses.AddRangeAsync(warehouses);

    var supplierNames = new[]
    {
        "Maple Industrial Supply", "Northern Trade Partners", "Great Lakes Components", "CanPro Distribution",
        "Prairie Equipment Co", "Atlantic Wholesale Group", "Summit Packaging Ltd", "Urban Office Source",
        "TrueNorth Safety", "Pacific Freight & Supply", "Evergreen Manufacturing Inputs", "Coreline Electric",
        "Metro Facility Supplies", "Pioneer Mechanical Parts", "Prime Logistics Vendor", "Vertex Process Materials",
        "Apex Distribution Network", "BlueWater Procurement", "Harborview Commerce", "Landmark Supplier Group",
        "Cedar Valley Imports", "Ironclad Tooling", "Diamond Warehousing Supply", "RedLeaf Consumables",
        "Skyline Vendor Services", "Northfield Materials", "Seaway Trade Corp", "Stonebridge Supplies",
        "Orchid Industrial Goods", "Nova Procurement Hub"
    };
    var suppliers = supplierNames.Select((name, i) => new Supplier
    {
        Name = name,
        ContactPerson = GetPersonDirectory()[(i + 9) % GetPersonDirectory().Count],
        Email = $"sales.{name.ToLowerInvariant().Replace(" ", "").Replace("&", "and")}.{runId}@example.com",
        PhoneNumber = $"+1-416-{(6000 + i):0000}",
        AddressLine1 = $"{200 + i} {Pick(random, GetStreetNames())}",
        City = cityDirectory[i % cityDirectory.Count].City,
        StateOrProvince = cityDirectory[i % cityDirectory.Count].Province,
        PostalCode = BuildCanadianPostalCode(random),
        Country = "Canada",
        IsActive = true,
        Notes = "Approved supplier account",
        CreatedBy = seedUser
    }).ToList();
    await context.Suppliers.AddRangeAsync(suppliers);
    await context.SaveChangesAsync();

    var products = new List<Product>(80);
    for (var i = 1; i <= 80; i++)
    {
        var uom = Pick(random, uoms);
        var category = Pick(random, categories);
        var brand = Pick(random, brands);
        var productFamily = Pick(random, GetProductFamilies());
        var modelCode = $"{productFamily.Code}-{random.Next(100, 999)}";

        products.Add(new Product
        {
            Name = $"{brand.Name} {productFamily.Name} {modelCode}",
            Description = $"{productFamily.Description} for {category.Name.ToLowerInvariant()} workflows",
            SKU = $"{productFamily.Code}-{i:0000}",
            Barcode = $"0{random.Next(100000000, 999999999)}{random.Next(1000, 9999)}",
            CategoryId = category.Id,
            BrandId = brand.Id,
            UnitOfMeasureId = uom.Id,
            CostPrice = NextDecimal(random, 5m, 80m),
            SellingPrice = NextDecimal(random, 20m, 200m),
            TaxRate = 13m,
            ReorderLevel = NextDecimal(random, 10m, 50m),
            ReorderQuantity = NextDecimal(random, 20m, 100m),
            TrackInventory = true,
            AllowBackOrder = true,
            IsSerialized = i % 7 == 0,
            IsBatchTracked = i % 5 == 0,
            IsPerishable = i % 8 == 0,
            Weight = NextDecimal(random, 0.2m, 15m),
            Length = NextDecimal(random, 5m, 60m),
            Width = NextDecimal(random, 5m, 50m),
            Height = NextDecimal(random, 2m, 40m),
            Status = ProductStatus.Active,
            ThumbnailImageUrl = $"https://cdn.seed.local/products/{i:000}.png",
            CreatedBy = seedUser
        });
    }
    await context.Products.AddRangeAsync(products);
    await context.SaveChangesAsync();

    var productSuppliers = new List<ProductSupplier>(160);
    for (var i = 0; i < 160; i++)
    {
        var product = Pick(random, products);
        var supplier = Pick(random, suppliers);

        if (productSuppliers.Any(x => x.ProductId == product.Id && x.SupplierId == supplier.Id))
        {
            continue;
        }

        productSuppliers.Add(new ProductSupplier
        {
            ProductId = product.Id,
            SupplierId = supplier.Id,
            SupplierProductCode = $"SPC-{runId}-{i:0000}",
            SupplierPrice = NextDecimal(random, 4m, 90m),
            LeadTimeInDays = random.Next(1, 21),
            IsPreferredSupplier = random.Next(0, 2) == 0,
            CreatedBy = seedUser
        });
    }
    await context.ProductSuppliers.AddRangeAsync(productSuppliers);

    var productImages = products.Take(80).Select((p, i) => new ProductImage
    {
        ProductId = p.Id,
        FileName = $"prod-{p.SKU}.png",
        FileUrl = $"https://cdn.seed.local/products/{p.SKU}.png",
        ContentType = "image/png",
        FileSizeInBytes = random.Next(20_000, 180_000),
        IsPrimary = true,
        DisplayOrder = 1,
        AltText = "Seed product image",
        CreatedBy = seedUser
    }).ToList();
    await context.ProductImages.AddRangeAsync(productImages);

    var inventoryStocks = new List<InventoryStock>(200);
    var seenStockKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    for (var i = 0; i < 200; i++)
    {
        var product = Pick(random, products);
        var warehouse = Pick(random, warehouses);
        var key = $"{product.Id:N}|{warehouse.Id:N}";
        if (!seenStockKeys.Add(key))
        {
            continue;
        }

        var onHand = NextDecimal(random, 20m, 500m);
        var reserved = NextDecimal(random, 0m, onHand / 2);
        var damaged = NextDecimal(random, 0m, onHand / 10);
        inventoryStocks.Add(new InventoryStock
        {
            ProductId = product.Id,
            WarehouseId = warehouse.Id,
            QuantityOnHand = onHand,
            QuantityReserved = reserved,
            QuantityAvailable = Math.Max(0m, onHand - reserved - damaged),
            QuantityDamaged = damaged,
            LastStockUpdatedAtUtc = DateTime.UtcNow.AddDays(-random.Next(0, 15)),
            CreatedBy = seedUser
        });
    }
    await context.InventoryStocks.AddRangeAsync(inventoryStocks);

    var stockMovements = inventoryStocks.Take(200).Select((stock, i) =>
    {
        var qtyBefore = NextDecimal(random, 20m, 300m);
        var qtyChange = NextDecimal(random, 1m, 40m);
        var isIn = random.Next(0, 2) == 0;
        var qtyAfter = isIn ? qtyBefore + qtyChange : Math.Max(0m, qtyBefore - qtyChange);
        return new StockMovement
        {
            ProductId = stock.ProductId,
            WarehouseId = stock.WarehouseId,
            MovementType = isIn ? StockMovementType.StockIn : StockMovementType.StockOut,
            Quantity = qtyChange,
            QuantityBefore = qtyBefore,
            QuantityAfter = qtyAfter,
            ReferenceNumber = $"STM-{runId}-{i:00000}",
            Reason = "Seed movement",
            Notes = "Seed stock movement",
            MovementDateUtc = DateTime.UtcNow.AddDays(-random.Next(0, 60)),
            CreatedBy = seedUser
        };
    }).ToList();
    await context.StockMovements.AddRangeAsync(stockMovements);

    await context.SaveChangesAsync();
    Console.WriteLine($"Seeded Inventory baseline: Products={products.Count}, Warehouses={warehouses.Count}, Stocks={inventoryStocks.Count}");
}

static async Task ImportInventoryFromWorkbookAsync(
    OperationIntelligenceDbContext context,
    string workbookPath,
    string seedUser)
{
    var sheets = ReadWorkbookSheets(workbookPath);

    static List<Dictionary<string, string>> Require(Dictionary<string, List<Dictionary<string, string>>> all, string name)
        => all.TryGetValue(name, out var rows) ? rows : new List<Dictionary<string, string>>();

    var categories = Require(sheets, "Categories")
        .Select(r => new Category
        {
            Id = ParseGuid(r, "Id"),
            Name = GetValue(r, "Name"),
            Description = GetValueOrNull(r, "Description"),
            ParentCategoryId = ParseGuidNullable(r, "ParentCategoryId"),
            CreatedBy = seedUser
        }).ToList();
    await context.Categories.AddRangeAsync(categories);

    var brands = Require(sheets, "Brands")
        .Select(r => new Brand
        {
            Id = ParseGuid(r, "Id"),
            Name = GetValue(r, "Name"),
            Description = GetValueOrNull(r, "Description"),
            CreatedBy = seedUser
        }).ToList();
    await context.Brands.AddRangeAsync(brands);

    var uoms = Require(sheets, "UnitOfMeasures")
        .Select(r => new UnitOfMeasure
        {
            Id = ParseGuid(r, "Id"),
            Name = GetValue(r, "Name"),
            Symbol = GetValue(r, "Symbol"),
            CreatedBy = seedUser
        }).ToList();
    await context.UnitsOfMeasure.AddRangeAsync(uoms);

    var warehouses = Require(sheets, "Warehouses")
        .Select(r => new Warehouse
        {
            Id = ParseGuid(r, "Id"),
            Name = GetValue(r, "Name"),
            Code = GetValue(r, "Code"),
            Description = GetValueOrNull(r, "Description"),
            AddressLine1 = GetValueOrNull(r, "AddressLine1"),
            AddressLine2 = GetValueOrNull(r, "AddressLine2"),
            City = GetValueOrNull(r, "City"),
            StateOrProvince = GetValueOrNull(r, "StateOrProvince"),
            PostalCode = GetValueOrNull(r, "PostalCode"),
            Country = GetValueOrNull(r, "Country"),
            IsActive = ParseBool(r, "IsActive", true),
            CreatedBy = seedUser
        }).ToList();
    await context.Warehouses.AddRangeAsync(warehouses);

    var suppliers = Require(sheets, "Suppliers")
        .Select(r => new Supplier
        {
            Id = ParseGuid(r, "Id"),
            Name = GetValue(r, "Name"),
            ContactPerson = GetValueOrNull(r, "ContactPerson"),
            Email = GetValueOrNull(r, "Email"),
            PhoneNumber = GetValueOrNull(r, "PhoneNumber"),
            AddressLine1 = GetValueOrNull(r, "AddressLine1"),
            AddressLine2 = GetValueOrNull(r, "AddressLine2"),
            City = GetValueOrNull(r, "City"),
            StateOrProvince = GetValueOrNull(r, "StateOrProvince"),
            PostalCode = GetValueOrNull(r, "PostalCode"),
            Country = GetValueOrNull(r, "Country"),
            IsActive = ParseBool(r, "IsActive", true),
            Notes = GetValueOrNull(r, "Notes"),
            CreatedBy = seedUser
        }).ToList();
    await context.Suppliers.AddRangeAsync(suppliers);

    var products = Require(sheets, "Products")
        .Select(r => new Product
        {
            Id = ParseGuid(r, "Id"),
            Name = GetValue(r, "Name"),
            Description = GetValueOrNull(r, "Description"),
            SKU = GetValue(r, "SKU"),
            Barcode = GetValueOrNull(r, "Barcode"),
            CategoryId = ParseGuid(r, "CategoryId"),
            BrandId = ParseGuidNullable(r, "BrandId"),
            UnitOfMeasureId = ParseGuid(r, "UnitOfMeasureId"),
            CostPrice = ParseDecimal(r, "CostPrice"),
            SellingPrice = ParseDecimal(r, "SellingPrice"),
            TaxRate = ParseDecimal(r, "TaxRate"),
            ReorderLevel = ParseDecimal(r, "ReorderLevel"),
            ReorderQuantity = ParseDecimal(r, "ReorderQuantity"),
            TrackInventory = ParseBool(r, "TrackInventory", true),
            AllowBackOrder = ParseBool(r, "AllowBackOrder", false),
            IsSerialized = ParseBool(r, "IsSerialized", false),
            IsBatchTracked = ParseBool(r, "IsBatchTracked", false),
            IsPerishable = ParseBool(r, "IsPerishable", false),
            Weight = ParseDecimal(r, "Weight"),
            Length = ParseDecimal(r, "Length"),
            Width = ParseDecimal(r, "Width"),
            Height = ParseDecimal(r, "Height"),
            Status = ParseEnum<ProductStatus>(r, "Status", ProductStatus.Active),
            ThumbnailImageUrl = GetValueOrNull(r, "ThumbnailImageUrl"),
            CreatedAtUtc = ParseExcelDateOrDefault(r, "CreatedAtUtc", DateTime.UtcNow),
            UpdatedAtUtc = ParseExcelDateNullable(r, "UpdatedAtUtc"),
            IsDeleted = ParseBool(r, "IsDeleted", false),
            DeletedAtUtc = ParseExcelDateNullable(r, "DeletedAtUtc"),
            CreatedBy = seedUser
        }).ToList();
    await context.Products.AddRangeAsync(products);
    await context.SaveChangesAsync();

    var productImages = Require(sheets, "ProductImages")
        .Select(r => new ProductImage
        {
            Id = ParseGuid(r, "Id"),
            ProductId = ParseGuid(r, "ProductId"),
            FileName = GetValue(r, "FileName"),
            FileUrl = GetValue(r, "FileUrl"),
            ContentType = GetValueOrNull(r, "ContentType"),
            FileSizeInBytes = ParseLong(r, "FileSizeInBytes"),
            IsPrimary = ParseBool(r, "IsPrimary", false),
            DisplayOrder = ParseInt(r, "DisplayOrder"),
            AltText = GetValueOrNull(r, "AltText"),
            CreatedAtUtc = ParseExcelDateOrDefault(r, "CreatedAtUtc", DateTime.UtcNow),
            UpdatedAtUtc = ParseExcelDateNullable(r, "UpdatedAtUtc"),
            IsDeleted = ParseBool(r, "IsDeleted", false),
            DeletedAtUtc = ParseExcelDateNullable(r, "DeletedAtUtc"),
            CreatedBy = seedUser
        }).ToList();
    await context.ProductImages.AddRangeAsync(productImages);

    var inventoryStocks = Require(sheets, "InventoryStocks")
        .Select(r => new InventoryStock
        {
            Id = ParseGuid(r, "Id"),
            ProductId = ParseGuid(r, "ProductId"),
            WarehouseId = ParseGuid(r, "WarehouseId"),
            QuantityOnHand = ParseDecimal(r, "QuantityOnHand"),
            QuantityReserved = ParseDecimal(r, "QuantityReserved"),
            QuantityAvailable = ParseDecimal(r, "QuantityAvailable"),
            QuantityDamaged = ParseDecimal(r, "QuantityDamaged"),
            LastStockUpdatedAtUtc = ParseExcelDateNullable(r, "LastStockUpdatedAtUtc"),
            CreatedAtUtc = ParseExcelDateOrDefault(r, "CreatedAtUtc", DateTime.UtcNow),
            UpdatedAtUtc = ParseExcelDateNullable(r, "UpdatedAtUtc"),
            IsDeleted = ParseBool(r, "IsDeleted", false),
            DeletedAtUtc = ParseExcelDateNullable(r, "DeletedAtUtc"),
            CreatedBy = seedUser
        }).ToList();
    await context.InventoryStocks.AddRangeAsync(inventoryStocks);

    var stockMovements = Require(sheets, "StockMovements")
        .Select(r => new StockMovement
        {
            Id = ParseGuid(r, "Id"),
            ProductId = ParseGuid(r, "ProductId"),
            WarehouseId = ParseGuid(r, "WarehouseId"),
            RelatedWarehouseId = ParseGuidNullable(r, "RelatedWarehouseId"),
            MovementType = ParseEnum<StockMovementType>(r, "MovementType", StockMovementType.StockIn),
            Quantity = ParseDecimal(r, "Quantity"),
            QuantityBefore = ParseDecimal(r, "QuantityBefore"),
            QuantityAfter = ParseDecimal(r, "QuantityAfter"),
            ReferenceNumber = GetValueOrNull(r, "ReferenceNumber"),
            Reason = GetValueOrNull(r, "Reason"),
            Notes = GetValueOrNull(r, "Notes"),
            MovementDateUtc = ParseExcelDateOrDefault(r, "MovementDateUtc", DateTime.UtcNow),
            CreatedAtUtc = ParseExcelDateOrDefault(r, "CreatedAtUtc", DateTime.UtcNow),
            UpdatedAtUtc = ParseExcelDateNullable(r, "UpdatedAtUtc"),
            IsDeleted = ParseBool(r, "IsDeleted", false),
            DeletedAtUtc = ParseExcelDateNullable(r, "DeletedAtUtc"),
            CreatedBy = seedUser
        }).ToList();
    await context.StockMovements.AddRangeAsync(stockMovements);

    var productSuppliers = Require(sheets, "ProductSuppliers")
        .Select(r => new ProductSupplier
        {
            Id = ParseGuid(r, "Id"),
            ProductId = ParseGuid(r, "ProductId"),
            SupplierId = ParseGuid(r, "SupplierId"),
            SupplierProductCode = GetValueOrNull(r, "SupplierProductCode"),
            SupplierPrice = ParseDecimal(r, "SupplierPrice"),
            LeadTimeInDays = ParseInt(r, "LeadTimeInDays"),
            IsPreferredSupplier = ParseBool(r, "IsPreferredSupplier", false),
            CreatedAtUtc = ParseExcelDateOrDefault(r, "CreatedAtUtc", DateTime.UtcNow),
            UpdatedAtUtc = ParseExcelDateNullable(r, "UpdatedAtUtc"),
            IsDeleted = ParseBool(r, "IsDeleted", false),
            DeletedAtUtc = ParseExcelDateNullable(r, "DeletedAtUtc"),
            CreatedBy = seedUser
        }).ToList();
    await context.ProductSuppliers.AddRangeAsync(productSuppliers);

    await context.SaveChangesAsync();
    Console.WriteLine($"Imported inventory workbook: Products={products.Count}, Warehouses={warehouses.Count}, Stocks={inventoryStocks.Count}");
}

static Dictionary<string, List<Dictionary<string, string>>> ReadWorkbookSheets(string workbookPath)
{
    var result = new Dictionary<string, List<Dictionary<string, string>>>(StringComparer.OrdinalIgnoreCase);

    using var zip = ZipFile.OpenRead(workbookPath);
    var nsMain = XNamespace.Get("http://schemas.openxmlformats.org/spreadsheetml/2006/main");
    var nsRel = XNamespace.Get("http://schemas.openxmlformats.org/officeDocument/2006/relationships");
    var nsPkgRel = XNamespace.Get("http://schemas.openxmlformats.org/package/2006/relationships");

    var workbookDoc = XDocument.Load(zip.GetEntry("xl/workbook.xml")!.Open());
    var relsDoc = XDocument.Load(zip.GetEntry("xl/_rels/workbook.xml.rels")!.Open());

    var relMap = relsDoc.Descendants(nsPkgRel + "Relationship")
        .Where(x => x.Attribute("Id") != null && x.Attribute("Target") != null)
        .ToDictionary(
            x => x.Attribute("Id")!.Value,
            x => x.Attribute("Target")!.Value,
            StringComparer.OrdinalIgnoreCase);

    var sheets = workbookDoc.Descendants(nsMain + "sheet")
        .Where(x => x.Attribute("name") != null && x.Attribute(nsRel + "id") != null)
        .Select(x => new
        {
            Name = x.Attribute("name")!.Value,
            RelId = x.Attribute(nsRel + "id")!.Value
        })
        .ToList();

    foreach (var sheet in sheets)
    {
        if (!relMap.TryGetValue(sheet.RelId, out var target)) continue;
        var normalized = target.TrimStart('/');
        if (!normalized.StartsWith("xl/", StringComparison.OrdinalIgnoreCase))
        {
            normalized = $"xl/{normalized}";
        }

        var entry = zip.GetEntry(normalized);
        if (entry == null) continue;

        var sheetDoc = XDocument.Load(entry.Open());
        var rows = sheetDoc.Descendants(nsMain + "row").ToList();
        if (rows.Count == 0) continue;

        var headers = rows[0].Descendants(nsMain + "c").Select(c => ReadCellText(c, nsMain)).ToList();
        var dataRows = new List<Dictionary<string, string>>();
        for (var i = 1; i < rows.Count; i++)
        {
            var row = rows[i];
            var cells = row.Descendants(nsMain + "c").Select(c => ReadCellText(c, nsMain)).ToList();
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            for (var c = 0; c < headers.Count; c++)
            {
                var key = headers[c];
                if (string.IsNullOrWhiteSpace(key)) continue;
                dict[key] = c < cells.Count ? cells[c] : string.Empty;
            }

            dataRows.Add(dict);
        }

        result[sheet.Name] = dataRows;
    }

    return result;
}

static string ReadCellText(XElement cell, XNamespace nsMain)
{
    var type = cell.Attribute("t")?.Value;
    if (string.Equals(type, "inlineStr", StringComparison.OrdinalIgnoreCase))
    {
        return cell.Descendants(nsMain + "t").FirstOrDefault()?.Value?.Trim() ?? string.Empty;
    }

    return cell.Element(nsMain + "v")?.Value?.Trim() ?? string.Empty;
}

static string GetValue(Dictionary<string, string> row, string column)
{
    var value = GetValueOrNull(row, column);
    return value ?? throw new InvalidOperationException($"Missing required column value '{column}'.");
}

static string? GetValueOrNull(Dictionary<string, string> row, string column)
{
    if (!row.TryGetValue(column, out var value)) return null;
    if (string.IsNullOrWhiteSpace(value)) return null;
    return value.Trim();
}

static Guid ParseGuid(Dictionary<string, string> row, string column)
{
    var value = GetValue(row, column);
    return Guid.TryParse(value, out var guid)
        ? guid
        : throw new InvalidOperationException($"Invalid GUID for column '{column}': {value}");
}

static Guid? ParseGuidNullable(Dictionary<string, string> row, string column)
{
    var value = GetValueOrNull(row, column);
    if (value == null) return null;
    return Guid.TryParse(value, out var guid) ? guid : null;
}

static bool ParseBool(Dictionary<string, string> row, string column, bool fallback)
{
    var value = GetValueOrNull(row, column);
    if (value == null) return fallback;
    if (value == "1") return true;
    if (value == "0") return false;
    return bool.TryParse(value, out var result) ? result : fallback;
}

static int ParseInt(Dictionary<string, string> row, string column)
{
    var value = GetValueOrNull(row, column);
    if (string.IsNullOrWhiteSpace(value)) return 0;
    if (int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var i)) return i;
    if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var d)) return (int)d;
    return 0;
}

static long ParseLong(Dictionary<string, string> row, string column)
{
    var value = GetValueOrNull(row, column);
    if (string.IsNullOrWhiteSpace(value)) return 0L;
    if (long.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var i)) return i;
    if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var d)) return (long)d;
    return 0L;
}

static decimal ParseDecimal(Dictionary<string, string> row, string column)
{
    var value = GetValueOrNull(row, column);
    if (string.IsNullOrWhiteSpace(value)) return 0m;
    return decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var d) ? d : 0m;
}

static TEnum ParseEnum<TEnum>(Dictionary<string, string> row, string column, TEnum fallback)
    where TEnum : struct, Enum
{
    var value = GetValueOrNull(row, column);
    if (string.IsNullOrWhiteSpace(value)) return fallback;

    if (Enum.TryParse<TEnum>(value, true, out var parsed))
    {
        return parsed;
    }

    return int.TryParse(value, out var i) && Enum.IsDefined(typeof(TEnum), i)
        ? (TEnum)Enum.ToObject(typeof(TEnum), i)
        : fallback;
}

static DateTime? ParseExcelDateNullable(Dictionary<string, string> row, string column)
{
    var value = GetValueOrNull(row, column);
    if (string.IsNullOrWhiteSpace(value)) return null;

    if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var oa))
    {
        return DateTime.FromOADate(oa);
    }

    return DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var parsed)
        ? parsed.ToUniversalTime()
        : null;
}

static DateTime ParseExcelDateOrDefault(Dictionary<string, string> row, string column, DateTime fallback)
{
    return ParseExcelDateNullable(row, column) ?? fallback;
}

static async Task<List<PlatformUser>> SeedUsersAsync(
    OperationIntelligenceDbContext context,
    Random random,
    string runId,
    string seedUser,
    int targetCount)
{
    var count = Math.Clamp(targetCount, 50, 120);
    var users = new List<PlatformUser>(count);
    var people = GetPersonDirectory();

    for (var i = 0; i < count; i++)
    {
        var person = people[i % people.Count];
        var cityInfo = Pick(random, GetCityDirectory());
        var firstName = person.Split(' ')[0];
        var lastName = person.Split(' ').Last();

        users.Add(new PlatformUser
        {
            FirstName = firstName,
            LastName = lastName,
            Email = $"{firstName.ToLowerInvariant()}.{lastName.ToLowerInvariant()}.{runId}.{i + 1}@northstar-ops.com",
            PasswordHash = "seed-password-hash",
            Avatar = string.Empty,
            Gender = i % 2 == 0 ? "Male" : "Female",
            PhoneNumber = $"+1-416-{(2000 + i):0000}",
            Address = $"{100 + i} {Pick(random, GetStreetNames())}",
            City = cityInfo.City,
            State = cityInfo.Province,
            Country = "Canada",
            PostalCode = BuildCanadianPostalCode(random)
        });
    }

    await context.Users.AddRangeAsync(users);
    await context.SaveChangesAsync();
    return users;
}

static async Task<(List<Order> Orders, List<OrderItem> OrderItems, List<OrderAddress> OrderAddresses)> SeedOrdersAsync(
    OperationIntelligenceDbContext context,
    Random random,
    string runId,
    string seedUser,
    int targetCount,
    IReadOnlyList<Product> products,
    IReadOnlyList<Warehouse> warehouses,
    IReadOnlyList<UnitOfMeasure> uoms)
{
    var orderCount = Math.Clamp(targetCount, 50, 200);
    var orderList = new List<Order>(orderCount);
    var customerCompanies = GetCustomerCompanies();
    var customerContacts = GetPersonDirectory();
    var cityDirectory = GetCityDirectory();

    for (var i = 0; i < orderCount; i++)
    {
        var orderDate = NextDate(random, DateTime.UtcNow.AddDays(-120), DateTime.UtcNow.AddDays(-2));
        var requiredDate = orderDate.AddDays(random.Next(3, 21));
        var company = customerCompanies[i % customerCompanies.Count];
        var contact = customerContacts[(i + 4) % customerContacts.Count];
        var firstName = contact.Split(' ')[0].ToLowerInvariant();
        var lastName = contact.Split(' ')[1].ToLowerInvariant();
        var cityInfo = cityDirectory[i % cityDirectory.Count];

        orderList.Add(new Order
        {
            OrderNumber = $"SO-{runId}-{i + 1:0000}",
            CustomerName = $"{contact} ({company})",
            CustomerEmail = $"{firstName}.{lastName}@{company.ToLowerInvariant().Replace(" ", "").Replace("&", "and")}.com",
            CustomerPhone = $"+1-437-{(3000 + i):0000}",
            OrderType = OrderType.Sales,
            Status = RandomEnum<OrderStatus>(random),
            Priority = RandomEnum<OrderPriority>(random),
            Channel = RandomEnum<OrderChannel>(random),
            WarehouseId = Pick(random, warehouses).Id,
            OrderDateUtc = orderDate,
            RequiredDateUtc = requiredDate,
            CurrencyCode = "CAD",
            ReferenceNumber = $"ERP-{runId}-{i + 1:0000}",
            CustomerPurchaseOrderNumber = $"PO-{cityInfo.City[..Math.Min(3, cityInfo.City.Length)].ToUpperInvariant()}-{i + 1:0000}",
            SalesPerson = GetPersonDirectory()[(i + 21) % GetPersonDirectory().Count],
            Notes = $"Standard fulfillment order for {company}",
            CreatedAtUtc = orderDate,
            CreatedBy = seedUser,
            IsActive = true
        });
    }

    await context.Orders.AddRangeAsync(orderList);
    await context.SaveChangesAsync();

    var orderAddresses = new List<OrderAddress>(orderCount * 2);
    foreach (var order in orderList)
    {
        var cityInfo = Pick(random, cityDirectory);
        var shippingCity = cityInfo.City;
        var shippingProvince = cityInfo.Province;
        var contact = order.CustomerName?.Split('(')[0].Trim() ?? "Operations Contact";
        var companyName = order.CustomerName?.Contains('(') == true
            ? order.CustomerName.Split('(')[1].Replace(")", "").Trim()
            : "Customer Operations";

        orderAddresses.Add(new OrderAddress
        {
            OrderId = order.Id,
            AddressType = AddressType.Billing,
            ContactName = contact,
            CompanyName = companyName,
            AddressLine1 = $"{random.Next(100, 999)} {Pick(random, GetStreetNames())}",
            City = shippingCity,
            StateOrProvince = shippingProvince,
            PostalCode = BuildCanadianPostalCode(random),
            Country = "Canada",
            PhoneNumber = order.CustomerPhone,
            Email = order.CustomerEmail,
            CreatedAtUtc = order.CreatedAtUtc,
            CreatedBy = seedUser
        });

        orderAddresses.Add(new OrderAddress
        {
            OrderId = order.Id,
            AddressType = AddressType.Shipping,
            ContactName = contact,
            CompanyName = companyName,
            AddressLine1 = $"{random.Next(100, 999)} {Pick(random, GetStreetNames())}",
            City = shippingCity,
            StateOrProvince = shippingProvince,
            PostalCode = BuildCanadianPostalCode(random),
            Country = "Canada",
            PhoneNumber = order.CustomerPhone,
            Email = order.CustomerEmail,
            CreatedAtUtc = order.CreatedAtUtc,
            CreatedBy = seedUser
        });
    }

    await context.OrderAddresses.AddRangeAsync(orderAddresses);

    var orderItems = new List<OrderItem>(orderCount * 3);
    var orderNotes = new List<OrderNote>(orderCount);
    var orderImages = new List<OrderImage>(orderCount);
    var orderStatusHistory = new List<OrderStatusHistory>(orderCount * 2);
    var orderPayments = new List<OrderPayment>(orderCount);

    for (var i = 0; i < orderList.Count; i++)
    {
        var order = orderList[i];
        var itemCount = random.Next(2, 5);
        decimal subtotal = 0m;
        decimal tax = 0m;

        for (var line = 1; line <= itemCount; line++)
        {
            var product = Pick(random, products);
            var orderedQty = NextDecimal(random, 5m, 60m);
            var allocatedQty = Math.Min(orderedQty, NextDecimal(random, 0m, orderedQty));
            var shippedQty = Math.Min(allocatedQty, NextDecimal(random, 0m, allocatedQty));
            var deliveredQty = Math.Min(shippedQty, NextDecimal(random, 0m, shippedQty));
            var cancelledQty = Math.Max(0m, orderedQty - allocatedQty);
            var unitPrice = product.SellingPrice <= 0 ? NextDecimal(random, 5m, 200m) : product.SellingPrice;
            var discount = NextDecimal(random, 0m, 10m);
            var lineTax = Math.Round((orderedQty * unitPrice - discount) * 0.13m, 2);
            var lineTotal = Math.Round((orderedQty * unitPrice) - discount + lineTax, 2);

            subtotal += Math.Round((orderedQty * unitPrice) - discount, 2);
            tax += lineTax;

            orderItems.Add(new OrderItem
            {
                OrderId = order.Id,
                ProductId = product.Id,
                UnitOfMeasureId = product.UnitOfMeasureId == Guid.Empty ? Pick(random, uoms).Id : product.UnitOfMeasureId,
                ProductNameSnapshot = product.Name,
                ProductSkuSnapshot = product.SKU,
                ProductDescriptionSnapshot = product.Description,
                QuantityOrdered = orderedQty,
                QuantityAllocated = allocatedQty,
                QuantityShipped = shippedQty,
                QuantityDelivered = deliveredQty,
                QuantityCancelled = cancelledQty,
                UnitPrice = unitPrice,
                DiscountAmount = discount,
                TaxAmount = lineTax,
                LineTotal = lineTotal,
                SortOrder = line,
                Remarks = "Seeded order item",
                CreatedAtUtc = order.CreatedAtUtc,
                CreatedBy = seedUser,
                IsActive = true
            });
        }

        var shipping = NextDecimal(random, 8m, 40m);
        var total = subtotal + tax + shipping;
        var paid = Math.Round(total * (decimal)random.NextDouble(), 2);

        order.SubtotalAmount = subtotal;
        order.TaxAmount = tax;
        order.DiscountAmount = 0m;
        order.ShippingAmount = shipping;
        order.TotalAmount = total;
        order.PaidAmount = paid;
        order.RefundedAmount = 0m;
        order.OutstandingAmount = total - paid;
        order.PaymentStatus = paid <= 0m
            ? PaymentStatus.Unpaid
            : paid >= total ? PaymentStatus.Paid : PaymentStatus.PartiallyPaid;

        orderNotes.Add(new OrderNote
        {
            OrderId = order.Id,
            Note = "Seed note",
            IsInternal = i % 2 == 0,
            CreatedAtUtc = order.CreatedAtUtc,
            CreatedBy = seedUser
        });

        orderImages.Add(new OrderImage
        {
            OrderId = order.Id,
            FileName = $"order-{order.OrderNumber}.png",
            OriginalFileName = "proof.png",
            FileExtension = ".png",
            ContentType = "image/png",
            FileSizeBytes = random.Next(20_000, 200_000),
            StoragePath = $"/seed/orders/{order.OrderNumber}.png",
            PublicUrl = $"https://cdn.seed.local/orders/{order.OrderNumber}.png",
            ImageType = OrderImageType.General,
            Caption = "Seed image",
            IsPrimary = true,
            UploadedAtUtc = order.CreatedAtUtc,
            UploadedBy = seedUser,
            IsActive = true,
            CreatedAtUtc = order.CreatedAtUtc,
            CreatedBy = seedUser
        });

        orderStatusHistory.Add(new OrderStatusHistory
        {
            OrderId = order.Id,
            FromStatus = OrderStatus.Draft,
            ToStatus = order.Status,
            ChangedAtUtc = order.CreatedAtUtc,
            ChangedBy = seedUser,
            Reason = "Seed status",
            Comments = "Initial seeded status",
            CreatedAtUtc = order.CreatedAtUtc,
            CreatedBy = seedUser
        });

        orderPayments.Add(new OrderPayment
        {
            OrderId = order.Id,
            PaymentReference = $"PAY-{runId}-{i + 1:0000}",
            PaymentMethod = RandomEnum<PaymentMethod>(random),
            PaymentProvider = PaymentProvider.Manual,
            TransactionType = PaymentTransactionType.Payment,
            Status = order.PaymentStatus == PaymentStatus.Unpaid ? PaymentStatus.Pending : PaymentStatus.Paid,
            Amount = Math.Max(0m, paid),
            FeeAmount = Math.Round(paid * 0.02m, 2),
            NetAmount = Math.Round(paid * 0.98m, 2),
            CurrencyCode = "CAD",
            PaymentDateUtc = order.CreatedAtUtc.AddHours(2),
            ProcessedDateUtc = order.CreatedAtUtc.AddHours(3),
            PayerName = order.CustomerName,
            PayerEmail = order.CustomerEmail,
            Last4 = $"{random.Next(1000, 9999)}",
            RecordedBy = seedUser,
            IsActive = true,
            CreatedAtUtc = order.CreatedAtUtc,
            CreatedBy = seedUser
        });
    }

    await context.OrderItems.AddRangeAsync(orderItems);
    await context.OrderNotes.AddRangeAsync(orderNotes);
    await context.OrderImages.AddRangeAsync(orderImages);
    await context.OrderStatusHistories.AddRangeAsync(orderStatusHistory);
    await context.OrderPayments.AddRangeAsync(orderPayments);
    context.Orders.UpdateRange(orderList);
    await context.SaveChangesAsync();

    Console.WriteLine($"Seeded Orders: {orderList.Count} / OrderItems: {orderItems.Count}");
    return (orderList, orderItems, orderAddresses);
}

static async Task<ProductionSeedResult> SeedProductionAsync(
    OperationIntelligenceDbContext context,
    Random random,
    string runId,
    string seedUser,
    int targetCount,
    IReadOnlyList<Product> products,
    IReadOnlyList<Warehouse> warehouses,
    IReadOnlyList<UnitOfMeasure> uoms,
    IReadOnlyList<PlatformUser> users,
    IReadOnlyList<OrderItem> orderItems)
{
    var workCenterCount = Math.Clamp(targetCount, 50, 120);
    var workCenters = Enumerable.Range(1, workCenterCount).Select(i => new WorkCenter
    {
        Code = $"WC-{runId}-{i:000}",
        Name = $"Work Center {i}",
        Description = "Seeded work center",
        WarehouseId = Pick(random, warehouses).Id,
        CapacityPerDay = NextDecimal(random, 200m, 900m),
        AvailableOperators = random.Next(2, 20),
        IsActive = true,
        CreatedBy = seedUser
    }).ToList();

    await context.WorkCenters.AddRangeAsync(workCenters);
    await context.SaveChangesAsync();

    var machineCount = Math.Clamp(targetCount * 2, 80, 200);
    var machines = Enumerable.Range(1, machineCount).Select(i => new Machine
    {
        MachineCode = $"MCH-{runId}-{i:000}",
        Name = $"Machine {i}",
        WorkCenterId = Pick(random, workCenters).Id,
        Manufacturer = "Seed Manufacturing",
        Model = $"SM-{random.Next(100, 999)}",
        SerialNumber = $"SN-{runId}-{i:0000}",
        HourlyRunningCost = NextDecimal(random, 20m, 180m),
        Status = RandomEnum<MachineStatus>(random),
        LastMaintenanceDate = DateTime.UtcNow.AddDays(-random.Next(1, 90)),
        NextMaintenanceDate = DateTime.UtcNow.AddDays(random.Next(7, 120)),
        IsActive = true,
        CreatedBy = seedUser
    }).ToList();

    await context.Machines.AddRangeAsync(machines);

    var routingCount = Math.Clamp(targetCount, 50, 140);
    var routings = new List<Routing>(routingCount);
    var routingVersionByProduct = await context.Routings
        .AsNoTracking()
        .GroupBy(x => x.ProductId)
        .Select(x => new { x.Key, MaxVersion = x.Max(v => v.Version) })
        .ToDictionaryAsync(x => x.Key, x => x.MaxVersion);

    for (var i = 1; i <= routingCount; i++)
    {
        var product = Pick(random, products);
        routingVersionByProduct.TryGetValue(product.Id, out var version);
        version += 1;
        routingVersionByProduct[product.Id] = version;

        routings.Add(new Routing
        {
            RoutingCode = $"RT-{runId}-{i:0000}",
            Name = $"Routing {i}",
            ProductId = product.Id,
            Version = version,
            IsActive = true,
            IsDefault = version == 1,
            EffectiveFrom = DateTime.UtcNow.AddDays(-random.Next(30, 240)),
            Notes = "Seed routing",
            CreatedBy = seedUser
        });
    }

    var bomCount = Math.Clamp(targetCount, 50, 140);
    var boms = new List<BillOfMaterial>(bomCount);
    var bomVersionByProduct = await context.BillsOfMaterial
        .AsNoTracking()
        .GroupBy(x => x.ProductId)
        .Select(x => new { x.Key, MaxVersion = x.Max(v => v.Version) })
        .ToDictionaryAsync(x => x.Key, x => x.MaxVersion);

    for (var i = 1; i <= bomCount; i++)
    {
        var product = Pick(random, products);
        bomVersionByProduct.TryGetValue(product.Id, out var version);
        version += 1;
        bomVersionByProduct[product.Id] = version;

        boms.Add(new BillOfMaterial
        {
            BomCode = $"BOM-{runId}-{i:0000}",
            Name = $"BOM {i}",
            ProductId = product.Id,
            BaseQuantity = NextDecimal(random, 1m, 10m),
            UnitOfMeasureId = product.UnitOfMeasureId == Guid.Empty ? Pick(random, uoms).Id : product.UnitOfMeasureId,
            Version = version,
            IsActive = true,
            IsDefault = version == 1,
            EffectiveFrom = DateTime.UtcNow.AddDays(-random.Next(60, 360)),
            Notes = "Seed BOM",
            CreatedBy = seedUser
        });
    }

    await context.Routings.AddRangeAsync(routings);
    await context.BillsOfMaterial.AddRangeAsync(boms);
    await context.SaveChangesAsync();

    var routingSteps = new List<RoutingStep>(Math.Clamp(targetCount * 3, 150, 200));
    foreach (var routing in routings)
    {
        var max = routingSteps.Count >= 200 ? 2 : 3;
        for (var seq = 1; seq <= max; seq++)
        {
            routingSteps.Add(new RoutingStep
            {
                RoutingId = routing.Id,
                Sequence = seq,
                OperationCode = $"OP-{seq:00}",
                OperationName = $"Operation {seq}",
                WorkCenterId = Pick(random, workCenters).Id,
                SetupTimeMinutes = NextDecimal(random, 10m, 40m),
                RunTimeMinutesPerUnit = NextDecimal(random, 1m, 8m),
                QueueTimeMinutes = NextDecimal(random, 0m, 20m),
                WaitTimeMinutes = NextDecimal(random, 0m, 15m),
                MoveTimeMinutes = NextDecimal(random, 0m, 10m),
                RequiredOperators = random.Next(1, 5),
                IsOutsourced = random.Next(0, 10) == 0,
                IsQualityCheckpointRequired = random.Next(0, 3) == 0,
                Instructions = "Seed instruction",
                Notes = "Seed step",
                CreatedBy = seedUser
            });

            if (routingSteps.Count >= 200) break;
        }

        if (routingSteps.Count >= 200) break;
    }

    var bomItems = new List<BillOfMaterialItem>(Math.Clamp(targetCount * 3, 150, 200));
    foreach (var bom in boms)
    {
        var max = bomItems.Count >= 200 ? 2 : 3;
        for (var seq = 1; seq <= max; seq++)
        {
            var material = Pick(random, products);
            bomItems.Add(new BillOfMaterialItem
            {
                BillOfMaterialId = bom.Id,
                MaterialProductId = material.Id,
                QuantityRequired = NextDecimal(random, 0.5m, 15m),
                UnitOfMeasureId = material.UnitOfMeasureId == Guid.Empty ? Pick(random, uoms).Id : material.UnitOfMeasureId,
                ScrapFactorPercent = NextDecimal(random, 0m, 5m),
                YieldFactorPercent = NextDecimal(random, 90m, 100m),
                IsOptional = random.Next(0, 10) == 0,
                IsBackflush = random.Next(0, 2) == 0,
                Sequence = seq,
                Notes = "Seed BOM item",
                CreatedBy = seedUser
            });

            if (bomItems.Count >= 200) break;
        }

        if (bomItems.Count >= 200) break;
    }

    await context.RoutingSteps.AddRangeAsync(routingSteps);
    await context.BillOfMaterialItems.AddRangeAsync(bomItems);
    await context.SaveChangesAsync();

    var productionOrderCount = Math.Clamp(targetCount * 2, 100, 200);
    var productionOrders = new List<ProductionOrder>(productionOrderCount);

    for (var i = 1; i <= productionOrderCount; i++)
    {
        var routing = Pick(random, routings);
        var bom = boms.FirstOrDefault(x => x.ProductId == routing.ProductId) ?? Pick(random, boms);
        var warehouse = Pick(random, warehouses);
        var quantity = NextDecimal(random, 20m, 500m);
        var sourceOrderItem = orderItems.Count > 0 ? Pick(random, orderItems) : null;
        var start = NextDate(random, DateTime.UtcNow.AddDays(-40), DateTime.UtcNow.AddDays(20));

        productionOrders.Add(new ProductionOrder
        {
            ProductionOrderNumber = $"MO-{runId}-{i:0000}",
            ProductId = routing.ProductId,
            PlannedQuantity = quantity,
            ProducedQuantity = Math.Round(quantity * (decimal)random.NextDouble(), 2),
            ScrapQuantity = NextDecimal(random, 0m, 10m),
            RemainingQuantity = NextDecimal(random, 0m, quantity / 2),
            UnitOfMeasureId = products.First(x => x.Id == routing.ProductId).UnitOfMeasureId,
            BillOfMaterialId = bom.Id,
            RoutingId = routing.Id,
            WarehouseId = warehouse.Id,
            PlannedStartDate = start,
            PlannedEndDate = start.AddDays(random.Next(1, 7)),
            ActualStartDate = random.Next(0, 2) == 0 ? start : null,
            ActualEndDate = null,
            Status = RandomEnum<ProductionOrderStatus>(random),
            Priority = RandomEnum<ProductionPriority>(random),
            SourceType = ProductionSourceType.SalesOrder,
            SourceReferenceId = sourceOrderItem?.OrderId,
            BatchNumber = $"B-{runId}-{i:0000}",
            LotNumber = $"L-{runId}-{i:0000}",
            Notes = "Seed production order",
            EstimatedMaterialCost = NextDecimal(random, 200m, 5000m),
            EstimatedLaborCost = NextDecimal(random, 100m, 2000m),
            EstimatedOverheadCost = NextDecimal(random, 50m, 1500m),
            ActualMaterialCost = NextDecimal(random, 100m, 4500m),
            ActualLaborCost = NextDecimal(random, 80m, 1800m),
            ActualOverheadCost = NextDecimal(random, 40m, 1200m),
            IsReleased = random.Next(0, 2) == 0,
            IsClosed = false,
            CreatedBy = seedUser
        });
    }

    await context.ProductionOrders.AddRangeAsync(productionOrders);
    await context.SaveChangesAsync();

    var stepsByRouting = routingSteps.GroupBy(x => x.RoutingId).ToDictionary(g => g.Key, g => g.ToList());

    var productionExecutions = new List<ProductionExecution>(Math.Clamp(targetCount * 2, 100, 200));
    for (var i = 0; i < productionExecutions.Capacity; i++)
    {
        var order = Pick(random, productionOrders);
        var steps = order.RoutingId.HasValue && stepsByRouting.TryGetValue(order.RoutingId.Value, out var value) ? value : routingSteps;
        var step = steps.Count > 0 ? Pick(random, steps) : null;
        var wc = step?.WorkCenterId != null
            ? workCenters.First(x => x.Id == step.WorkCenterId)
            : Pick(random, workCenters);

        var start = NextDate(random, DateTime.UtcNow.AddDays(-20), DateTime.UtcNow.AddDays(15));
        productionExecutions.Add(new ProductionExecution
        {
            ProductionOrderId = order.Id,
            RoutingStepId = step?.Id,
            WorkCenterId = wc.Id,
            MachineId = machines.Where(x => x.WorkCenterId == wc.Id).OrderBy(_ => Guid.NewGuid()).Select(x => (Guid?)x.Id).FirstOrDefault(),
            PlannedQuantity = NextDecimal(random, 10m, order.PlannedQuantity),
            CompletedQuantity = NextDecimal(random, 0m, order.PlannedQuantity),
            ScrapQuantity = NextDecimal(random, 0m, 5m),
            PlannedStartDate = start,
            PlannedEndDate = start.AddHours(random.Next(2, 18)),
            ActualStartDate = start,
            ActualEndDate = start.AddHours(random.Next(2, 20)),
            ActualSetupTimeMinutes = NextDecimal(random, 10m, 90m),
            ActualRunTimeMinutes = NextDecimal(random, 60m, 480m),
            ActualDowntimeMinutes = NextDecimal(random, 0m, 60m),
            Status = RandomEnum<ExecutionStatus>(random),
            Remarks = "Seed execution",
            CreatedBy = seedUser
        });
    }

    await context.ProductionExecutions.AddRangeAsync(productionExecutions);
    await context.SaveChangesAsync();

    var productionMaterialIssues = new List<ProductionMaterialIssue>(Math.Clamp(targetCount * 2, 100, 200));
    for (var i = 0; i < productionMaterialIssues.Capacity; i++)
    {
        var po = Pick(random, productionOrders);
        var product = Pick(random, products);
        var qty = NextDecimal(random, 5m, 100m);

        productionMaterialIssues.Add(new ProductionMaterialIssue
        {
            ProductionOrderId = po.Id,
            MaterialProductId = product.Id,
            WarehouseId = po.WarehouseId,
            PlannedQuantity = qty,
            IssuedQuantity = NextDecimal(random, 1m, qty),
            ReturnedQuantity = NextDecimal(random, 0m, 5m),
            UnitOfMeasureId = product.UnitOfMeasureId,
            BatchNumber = $"PMI-B-{runId}-{i:0000}",
            LotNumber = $"PMI-L-{runId}-{i:0000}",
            IssueDate = NextDate(random, DateTime.UtcNow.AddDays(-20), DateTime.UtcNow),
            Notes = "Seed material issue",
            CreatedBy = seedUser
        });
    }

    await context.ProductionMaterialIssues.AddRangeAsync(productionMaterialIssues);
    await context.SaveChangesAsync();

    var productionMaterialConsumptions = productionMaterialIssues
        .Take(Math.Clamp(targetCount * 2, 100, 200))
        .Select((issue, i) => new ProductionMaterialConsumption
        {
            ProductionMaterialIssueId = issue.Id,
            ProductionExecutionId = productionExecutions.Count == 0 ? null : Pick(random, productionExecutions).Id,
            ConsumedQuantity = NextDecimal(random, 1m, issue.IssuedQuantity <= 0 ? 1m : issue.IssuedQuantity),
            ConsumptionDate = NextDate(random, DateTime.UtcNow.AddDays(-20), DateTime.UtcNow),
            Notes = "Seed consumption",
            CreatedBy = seedUser
        }).ToList();

    var productionOutputs = productionOrders.Take(Math.Clamp(targetCount * 2, 100, 200)).Select((po, i) => new ProductionOutput
    {
        ProductionOrderId = po.Id,
        ProductId = po.ProductId,
        WarehouseId = po.WarehouseId,
        QuantityProduced = NextDecimal(random, 1m, po.PlannedQuantity),
        UnitOfMeasureId = po.UnitOfMeasureId,
        BatchNumber = $"POUT-B-{runId}-{i:0000}",
        LotNumber = $"POUT-L-{runId}-{i:0000}",
        OutputDate = NextDate(random, DateTime.UtcNow.AddDays(-20), DateTime.UtcNow),
        IsFinalOutput = random.Next(0, 2) == 0,
        Notes = "Seed output",
        CreatedBy = seedUser
    }).ToList();

    var productionScraps = productionOrders.Take(Math.Clamp(targetCount * 2, 100, 200)).Select((po, i) => new ProductionScrap
    {
        ProductionOrderId = po.Id,
        ProductionExecutionId = productionExecutions.Count == 0 ? null : Pick(random, productionExecutions).Id,
        ProductId = po.ProductId,
        ScrapQuantity = NextDecimal(random, 0.1m, 10m),
        UnitOfMeasureId = po.UnitOfMeasureId,
        Reason = RandomEnum<ScrapReasonType>(random),
        ReasonDescription = "Seed scrap",
        ScrapDate = NextDate(random, DateTime.UtcNow.AddDays(-20), DateTime.UtcNow),
        IsReworkable = random.Next(0, 2) == 0,
        Notes = "Seed scrap",
        CreatedBy = seedUser
    }).ToList();

    var productionDowntimes = productionExecutions.Take(Math.Clamp(targetCount * 2, 100, 200)).Select((exec, i) =>
    {
        var start = NextDate(random, DateTime.UtcNow.AddDays(-20), DateTime.UtcNow);
        var end = start.AddMinutes(random.Next(10, 180));
        return new ProductionDowntime
        {
            ProductionExecutionId = exec.Id,
            Reason = RandomEnum<DowntimeReasonType>(random),
            ReasonDescription = "Seed downtime",
            StartTime = start,
            EndTime = end,
            DurationMinutes = (decimal)(end - start).TotalMinutes,
            IsPlanned = random.Next(0, 2) == 0,
            Notes = "Seed downtime",
            CreatedBy = seedUser
        };
    }).ToList();

    var productionLaborLogs = productionExecutions.Take(Math.Clamp(targetCount * 2, 100, 200)).Select((exec, i) => new ProductionLaborLog
    {
        ProductionExecutionId = exec.Id,
        UserId = Pick(random, users).Id,
        HoursWorked = NextDecimal(random, 1m, 12m),
        HourlyRate = NextDecimal(random, 18m, 55m),
        WorkDate = NextDate(random, DateTime.UtcNow.AddDays(-30), DateTime.UtcNow),
        Notes = "Seed labor",
        CreatedBy = seedUser
    }).ToList();

    var productionQualityChecks = productionOrders.Take(Math.Clamp(targetCount * 2, 100, 200)).Select((po, i) => new ProductionQualityCheck
    {
        ProductionOrderId = po.Id,
        ProductionExecutionId = productionExecutions.Count == 0 ? null : Pick(random, productionExecutions).Id,
        CheckType = RandomEnum<QualityCheckType>(random),
        Status = RandomEnum<QualityCheckStatus>(random),
        CheckDate = NextDate(random, DateTime.UtcNow.AddDays(-20), DateTime.UtcNow),
        CheckedByUserId = Pick(random, users).Id,
        ReferenceStandard = "ISO-9001",
        Findings = "Seed findings",
        CorrectiveAction = "Seed action",
        RequiresRework = random.Next(0, 5) == 0,
        Notes = "Seed QC",
        CreatedBy = seedUser
    }).ToList();

    await context.ProductionMaterialConsumptions.AddRangeAsync(productionMaterialConsumptions);
    await context.ProductionOutputs.AddRangeAsync(productionOutputs);
    await context.ProductionScraps.AddRangeAsync(productionScraps);
    await context.ProductionDowntimes.AddRangeAsync(productionDowntimes);
    await context.ProductionLaborLogs.AddRangeAsync(productionLaborLogs);
    await context.ProductionQualityChecks.AddRangeAsync(productionQualityChecks);
    await context.SaveChangesAsync();

    Console.WriteLine($"Seeded Production: WorkCenters={workCenters.Count}, Orders={productionOrders.Count}, Executions={productionExecutions.Count}");

    return new ProductionSeedResult(
        workCenters,
        machines,
        routings,
        routingSteps,
        boms,
        productionOrders,
        productionExecutions);
}

static async Task<SchedulingSeedResult> SeedSchedulingAsync(
    OperationIntelligenceDbContext context,
    Random random,
    string runId,
    string seedUser,
    int targetCount,
    IReadOnlyList<Product> products,
    IReadOnlyList<Warehouse> warehouses,
    ProductionSeedResult production,
    IReadOnlyList<Order> orders,
    IReadOnlyList<OrderItem> orderItems)
{
    var shiftCount = Math.Clamp(targetCount + 20, 60, 200);
    var shifts = new List<Shift>(shiftCount);

    for (var i = 1; i <= shiftCount; i++)
    {
        var warehouse = Pick(random, warehouses);
        var wc = random.Next(0, 2) == 0 ? Pick(random, production.WorkCenters) : null;

        var startHour = new[] { 6, 14, 22 }[i % 3];
        var startTime = TimeSpan.FromHours(startHour);
        var endTime = TimeSpan.FromHours((startHour + 8) % 24);

        shifts.Add(new Shift
        {
            WarehouseId = warehouse.Id,
            WorkCenterId = wc?.Id,
            ShiftCode = $"SH-{runId}-{i:000}",
            ShiftName = $"Shift {i}",
            StartTime = startTime,
            EndTime = endTime,
            CrossesMidnight = startHour == 22,
            IsActive = true,
            CapacityMinutes = 480,
            BreakMinutes = 30,
            CreatedBy = seedUser
        });
    }

    await context.Shifts.AddRangeAsync(shifts);

    var resourceCalendars = new List<ResourceCalendar>(Math.Clamp(targetCount * 2, 100, 200));
    for (var i = 1; i <= resourceCalendars.Capacity; i++)
    {
        var useMachine = i % 2 == 0;
        var resourceId = useMachine ? Pick(random, production.Machines).Id : Pick(random, production.WorkCenters).Id;
        var resourceType = useMachine ? ResourceType.Machine : ResourceType.WorkCenter;

        resourceCalendars.Add(new ResourceCalendar
        {
            ResourceId = resourceId,
            ResourceType = resourceType,
            CalendarName = $"Calendar {i}",
            TimeZone = "America/Toronto",
            MondayEnabled = true,
            TuesdayEnabled = true,
            WednesdayEnabled = true,
            ThursdayEnabled = true,
            FridayEnabled = true,
            SaturdayEnabled = i % 4 == 0,
            SundayEnabled = false,
            DefaultStartTime = TimeSpan.FromHours(8),
            DefaultEndTime = TimeSpan.FromHours(16),
            IsDefault = i % 5 == 0,
            CreatedBy = seedUser
        });
    }

    await context.ResourceCalendars.AddRangeAsync(resourceCalendars);
    await context.SaveChangesAsync();

    var calendarExceptions = resourceCalendars.Take(Math.Clamp(targetCount * 2, 100, 200)).Select((cal, i) =>
    {
        var start = NextDate(random, DateTime.UtcNow.AddDays(-20), DateTime.UtcNow.AddDays(30));
        return new ResourceCalendarException
        {
            ResourceCalendarId = cal.Id,
            ExceptionStartUtc = start,
            ExceptionEndUtc = start.AddHours(random.Next(2, 12)),
            ExceptionType = RandomEnum<CalendarExceptionType>(random),
            IsWorkingException = random.Next(0, 2) == 0,
            Reason = "Seed calendar exception",
            Notes = "Seed calendar exception",
            CreatedBy = seedUser
        };
    }).ToList();

    var schedulePlans = new List<SchedulePlan>(Math.Clamp(targetCount, 50, 120));
    for (var i = 1; i <= schedulePlans.Capacity; i++)
    {
        var start = NextDate(random, DateTime.UtcNow.AddDays(-10), DateTime.UtcNow.AddDays(30));
        schedulePlans.Add(new SchedulePlan
        {
            PlanNumber = $"PLAN-{runId}-{i:0000}",
            Name = $"Schedule Plan {i}",
            Description = "Seed schedule plan",
            WarehouseId = Pick(random, warehouses).Id,
            PlanningStartDateUtc = start,
            PlanningEndDateUtc = start.AddDays(random.Next(5, 21)),
            Status = RandomEnum<SchedulePlanStatus>(random),
            GenerationMode = RandomEnum<ScheduleGenerationMode>(random),
            SchedulingStrategy = SchedulingStrategy.FiniteCapacity,
            AutoSequenceEnabled = true,
            AutoDispatchEnabled = i % 3 == 0,
            VersionNumber = 1,
            TimeZone = "America/Toronto",
            IsActive = true,
            CreatedBy = seedUser
        });
    }

    await context.ResourceCalendarExceptions.AddRangeAsync(calendarExceptions);
    await context.SchedulePlans.AddRangeAsync(schedulePlans);
    await context.SaveChangesAsync();

    var scheduleRevisions = new List<ScheduleRevision>(Math.Clamp(targetCount * 2, 100, 200));
    foreach (var plan in schedulePlans)
    {
        for (var rev = 1; rev <= 2; rev++)
        {
            scheduleRevisions.Add(new ScheduleRevision
            {
                SchedulePlanId = plan.Id,
                RevisionNo = rev,
                RevisionType = rev == 1 ? "Initial" : "Adjustment",
                ChangeSummary = "Seed revision",
                Reason = "Seeded",
                RevisedAtUtc = plan.PlanningStartDateUtc.AddDays(rev),
                SnapshotJson = "{}",
                CreatedBy = seedUser
            });

            if (scheduleRevisions.Count >= 200) break;
        }

        if (scheduleRevisions.Count >= 200) break;
    }

    var scheduleJobs = new List<ScheduleJob>(Math.Clamp(targetCount * 2, 100, 200));
    for (var i = 1; i <= scheduleJobs.Capacity; i++)
    {
        var plan = Pick(random, schedulePlans);
        var po = Pick(random, production.ProductionOrders);
        var oi = orderItems.Count > 0 ? Pick(random, orderItems) : null;

        scheduleJobs.Add(new ScheduleJob
        {
            SchedulePlanId = plan.Id,
            ProductionOrderId = po.Id,
            OrderId = oi?.OrderId,
            OrderItemId = oi?.Id,
            ProductId = po.ProductId,
            WarehouseId = po.WarehouseId,
            JobNumber = $"JOB-{runId}-{i:0000}",
            JobName = $"Schedule Job {i}",
            Notes = "Seed job",
            PlannedQuantity = po.PlannedQuantity,
            CompletedQuantity = NextDecimal(random, 0m, po.PlannedQuantity),
            ScrappedQuantity = NextDecimal(random, 0m, 10m),
            EarliestStartUtc = plan.PlanningStartDateUtc,
            LatestFinishUtc = plan.PlanningEndDateUtc,
            DueDateUtc = plan.PlanningEndDateUtc,
            Priority = RandomEnum<SchedulePriority>(random),
            Status = RandomEnum<ScheduleJobStatus>(random),
            MaterialsReady = random.Next(0, 2) == 0,
            MaterialReadinessStatus = RandomEnum<MaterialReadinessStatus>(random),
            MaterialsCheckedAtUtc = DateTime.UtcNow.AddDays(-random.Next(1, 10)),
            QualityHold = random.Next(0, 10) == 0,
            IsRushOrder = random.Next(0, 5) == 0,
            CreatedBy = seedUser
        });
    }

    await context.ScheduleRevisions.AddRangeAsync(scheduleRevisions);
    await context.ScheduleJobs.AddRangeAsync(scheduleJobs);
    await context.SaveChangesAsync();

    var opsByJob = new Dictionary<Guid, List<ScheduleOperation>>();
    var scheduleOperations = new List<ScheduleOperation>(Math.Clamp(targetCount * 3, 150, 200));

    for (var i = 0; i < scheduleJobs.Count; i++)
    {
        var job = scheduleJobs[i];
        var po = production.ProductionOrders.First(x => x.Id == job.ProductionOrderId);
        var steps = po.RoutingId.HasValue
            ? production.RoutingSteps.Where(x => x.RoutingId == po.RoutingId.Value).OrderBy(x => x.Sequence).ToList()
            : production.RoutingSteps.OrderBy(x => x.Sequence).Take(2).ToList();

        if (steps.Count == 0)
        {
            steps = production.RoutingSteps.Take(2).ToList();
        }

        var plannedStart = job.EarliestStartUtc ?? DateTime.UtcNow;
        var opCount = i < scheduleJobs.Count / 2 ? 2 : 1;

        for (var seq = 1; seq <= opCount; seq++)
        {
            if (scheduleOperations.Count >= 200) break;

            var step = steps[Math.Min(seq - 1, steps.Count - 1)];
            var start = plannedStart.AddHours((seq - 1) * 3);
            var end = start.AddHours(random.Next(2, 6));
            var machineId = production.Machines.Where(m => m.WorkCenterId == step.WorkCenterId).Select(m => (Guid?)m.Id).FirstOrDefault();
            var shift = shifts.Where(s => s.WorkCenterId == step.WorkCenterId || s.WarehouseId == job.WarehouseId).OrderBy(_ => Guid.NewGuid()).FirstOrDefault();

            var op = new ScheduleOperation
            {
                ScheduleJobId = job.Id,
                RoutingStepId = step.Id,
                WorkCenterId = step.WorkCenterId,
                MachineId = machineId,
                ProductionExecutionId = production.ProductionExecutions.Count == 0 ? null : Pick(random, production.ProductionExecutions).Id,
                PlannedShiftId = shift?.Id,
                ActualShiftId = shift?.Id,
                SequenceNo = seq,
                OperationCode = step.OperationCode,
                OperationName = step.OperationName,
                PlannedStartUtc = start,
                PlannedEndUtc = end,
                ActualStartUtc = start,
                ActualEndUtc = end,
                SetupTimeMinutes = step.SetupTimeMinutes,
                RunTimeMinutes = step.RunTimeMinutesPerUnit * job.PlannedQuantity,
                QueueTimeMinutes = step.QueueTimeMinutes,
                WaitTimeMinutes = step.WaitTimeMinutes,
                MoveTimeMinutes = step.MoveTimeMinutes,
                PlannedQuantity = job.PlannedQuantity,
                CompletedQuantity = NextDecimal(random, 0m, job.PlannedQuantity),
                ScrappedQuantity = NextDecimal(random, 0m, 8m),
                Status = RandomEnum<ScheduleOperationStatus>(random),
                DispatchStatus = RandomEnum<DispatchStatus>(random),
                IsCriticalPath = seq == 1,
                IsBottleneckOperation = random.Next(0, 5) == 0,
                IsOutsourced = step.IsOutsourced,
                PriorityScore = random.Next(1, 100),
                ConstraintReason = "Seed",
                Notes = "Seed operation",
                CreatedBy = seedUser
            };

            scheduleOperations.Add(op);
            if (!opsByJob.TryGetValue(job.Id, out var list))
            {
                list = new List<ScheduleOperation>();
                opsByJob[job.Id] = list;
            }

            list.Add(op);
        }

        if (scheduleOperations.Count >= 200) break;
    }

    await context.ScheduleOperations.AddRangeAsync(scheduleOperations);
    await context.SaveChangesAsync();

    var scheduleDependencies = new List<ScheduleOperationDependency>();
    foreach (var (_, ops) in opsByJob)
    {
        if (ops.Count < 2) continue;
        scheduleDependencies.Add(new ScheduleOperationDependency
        {
            PredecessorOperationId = ops[0].Id,
            SuccessorOperationId = ops[1].Id,
            DependencyType = DependencyType.FinishToStart,
            LagMinutes = random.Next(0, 30),
            IsMandatory = true,
            CreatedBy = seedUser
        });

        if (scheduleDependencies.Count >= 200) break;
    }

    var scheduleConstraints = scheduleOperations.Take(200).Select((op, i) => new ScheduleOperationConstraint
    {
        ScheduleOperationId = op.Id,
        ConstraintType = RandomEnum<OperationConstraintType>(random),
        ReferenceNo = $"C-{runId}-{i:0000}",
        Description = "Seed constraint",
        IsSatisfied = random.Next(0, 2) == 0,
        SatisfiedAtUtc = DateTime.UtcNow,
        IsMandatory = true,
        CreatedBy = seedUser
    }).ToList();

    var scheduleResourceOptions = scheduleOperations.Take(200).Select((op, i) =>
    {
        var useMachine = random.Next(0, 2) == 0;
        var resourceId = useMachine
            ? (op.MachineId ?? Pick(random, production.Machines).Id)
            : op.WorkCenterId;

        return new ScheduleOperationResourceOption
        {
            ScheduleOperationId = op.Id,
            ResourceId = resourceId,
            ResourceType = useMachine ? ResourceType.Machine : ResourceType.WorkCenter,
            IsPrimaryOption = true,
            PreferenceRank = 1,
            EfficiencyFactor = NextDecimal(random, 0.8m, 1.2m),
            SetupPenaltyMinutes = NextDecimal(random, 0m, 20m),
            IsActive = true,
            CreatedBy = seedUser
        };
    }).ToList();

    var scheduleResourceAssignments = scheduleOperations.Take(200).Select((op, i) =>
    {
        var shift = shifts.FirstOrDefault(x => x.Id == op.PlannedShiftId) ?? Pick(random, shifts);
        return new ScheduleResourceAssignment
        {
            ScheduleOperationId = op.Id,
            ResourceId = op.WorkCenterId,
            ResourceType = ResourceType.WorkCenter,
            ShiftId = shift.Id,
            AssignmentRole = "Primary",
            IsPrimary = true,
            AssignedStartUtc = op.PlannedStartUtc,
            AssignedEndUtc = op.PlannedEndUtc,
            PlannedHours = (decimal)(op.PlannedEndUtc - op.PlannedStartUtc).TotalHours,
            ActualHours = (decimal)(op.PlannedEndUtc - op.PlannedStartUtc).TotalHours,
            Status = AssignmentStatus.Planned,
            Notes = "Seed assignment",
            CreatedBy = seedUser
        };
    }).ToList();

    var capacityReservations = scheduleOperations.Take(200).Select((op, i) =>
    {
        var shift = shifts.FirstOrDefault(x => x.Id == op.PlannedShiftId);
        var minutes = (int)Math.Max(1, (op.PlannedEndUtc - op.PlannedStartUtc).TotalMinutes);
        return new CapacityReservation
        {
            ScheduleOperationId = op.Id,
            ResourceId = op.WorkCenterId,
            ResourceType = ResourceType.WorkCenter,
            ShiftId = shift?.Id,
            ReservedStartUtc = op.PlannedStartUtc,
            ReservedEndUtc = op.PlannedEndUtc,
            ReservedMinutes = minutes,
            AvailableMinutesAtBooking = Math.Max(minutes + 60, 480),
            Status = CapacityReservationStatus.Reserved,
            ReservationReason = "Seed reservation",
            CreatedBy = seedUser
        };
    }).ToList();

    var dispatchQueue = scheduleOperations.Take(200).Select((op, i) => new DispatchQueueItem
    {
        ScheduleOperationId = op.Id,
        WorkCenterId = op.WorkCenterId,
        MachineId = op.MachineId,
        QueuePosition = (i % 20) + 1,
        PriorityScore = op.PriorityScore,
        Status = RandomEnum<DispatchStatus>(random),
        ReleasedAtUtc = op.PlannedStartUtc,
        AcknowledgedAtUtc = op.PlannedStartUtc.AddMinutes(10),
        DispatchNotes = "Seed dispatch",
        IsActive = true,
        CreatedBy = seedUser
    }).ToList();

    var materialChecks = scheduleJobs.Take(Math.Clamp(targetCount * 2, 100, 200)).Select((job, i) =>
    {
        var op = opsByJob.TryGetValue(job.Id, out var ops) && ops.Count > 0 ? ops[0] : null;
        var product = Pick(random, products);
        var required = NextDecimal(random, 2m, 80m);
        var available = NextDecimal(random, 0m, required);
        return new ScheduleMaterialCheck
        {
            ScheduleJobId = job.Id,
            ScheduleOperationId = op?.Id,
            ProductionOrderId = job.ProductionOrderId,
            RoutingStepId = op?.RoutingStepId,
            MaterialProductId = product.Id,
            WarehouseId = job.WarehouseId,
            RequiredQuantity = required,
            AvailableQuantity = available,
            ReservedQuantity = NextDecimal(random, 0m, available),
            ShortageQuantity = Math.Max(0m, required - available),
            Status = available >= required ? MaterialReadinessStatus.Ready : MaterialReadinessStatus.Shortage,
            ExpectedAvailabilityDateUtc = DateTime.UtcNow.AddDays(random.Next(1, 10)),
            Notes = "Seed material check",
            CheckedAtUtc = DateTime.UtcNow,
            CreatedBy = seedUser
        };
    }).ToList();

    var scheduleExceptions = scheduleOperations.Take(Math.Clamp(targetCount * 2, 100, 200)).Select((op, i) => new ScheduleException
    {
        SchedulePlanId = scheduleJobs.First(x => x.Id == op.ScheduleJobId).SchedulePlanId,
        ScheduleJobId = op.ScheduleJobId,
        ScheduleOperationId = op.Id,
        ExceptionType = RandomEnum<ScheduleExceptionType>(random),
        Severity = RandomEnum<ScheduleExceptionSeverity>(random),
        Title = "Seed schedule exception",
        Description = "Seed schedule exception description",
        DetectedAtUtc = DateTime.UtcNow.AddDays(-random.Next(0, 10)),
        AssignedTo = "planner",
        Status = RandomEnum<ScheduleExceptionStatus>(random),
        ResolutionNotes = "Seed resolution",
        CreatedBy = seedUser
    }).ToList();

    var scheduleRescheduleHistories = scheduleOperations.Take(Math.Clamp(targetCount * 2, 100, 200)).Select((op, i) => new ScheduleRescheduleHistory
    {
        SchedulePlanId = scheduleJobs.First(x => x.Id == op.ScheduleJobId).SchedulePlanId,
        ScheduleJobId = op.ScheduleJobId,
        ScheduleOperationId = op.Id,
        OldPlannedStartUtc = op.PlannedStartUtc.AddHours(-1),
        OldPlannedEndUtc = op.PlannedEndUtc.AddHours(-1),
        NewPlannedStartUtc = op.PlannedStartUtc,
        NewPlannedEndUtc = op.PlannedEndUtc,
        OldWorkCenterId = op.WorkCenterId,
        NewWorkCenterId = op.WorkCenterId,
        OldMachineId = op.MachineId,
        NewMachineId = op.MachineId,
        ReasonCode = "PRIORITY_CHANGE",
        ReasonDescription = "Seeded reschedule",
        ChangedAtUtc = DateTime.UtcNow,
        CreatedBy = seedUser
    }).ToList();

    var scheduleStatusHistories = scheduleOperations.Take(200).Select((op, i) => new ScheduleStatusHistory
    {
        SchedulePlanId = scheduleJobs.First(x => x.Id == op.ScheduleJobId).SchedulePlanId,
        ScheduleJobId = op.ScheduleJobId,
        ScheduleOperationId = op.Id,
        EntityType = "ScheduleOperation",
        OldStatus = ScheduleOperationStatus.Pending.ToString(),
        NewStatus = op.Status.ToString(),
        Reason = "Seed",
        Notes = "Seed status history",
        ChangedAtUtc = DateTime.UtcNow,
        CreatedBy = seedUser
    }).ToList();

    var scheduleAuditLogs = Enumerable.Range(1, Math.Clamp(targetCount * 2, 100, 200)).Select(i => new ScheduleAuditLog
    {
        EntityName = i % 2 == 0 ? "ScheduleJob" : "ScheduleOperation",
        EntityId = i % 2 == 0 ? Pick(random, scheduleJobs).Id : Pick(random, scheduleOperations).Id,
        ActionType = "UPDATE",
        ChangedFieldsJson = "[\"Status\",\"PlannedStartUtc\"]",
        OldValuesJson = "{}",
        NewValuesJson = "{}",
        Source = "Seeder",
        Reason = "Seeded audit",
        PerformedAtUtc = DateTime.UtcNow,
        CorrelationId = Guid.NewGuid().ToString("N"),
        CreatedBy = seedUser
    }).ToList();

    var resourceCapacitySnapshots = Enumerable.Range(1, Math.Clamp(targetCount * 2, 100, 200)).Select(i =>
    {
        var shift = Pick(random, shifts);
        var isMachine = i % 2 == 0;
        var resourceId = isMachine ? Pick(random, production.Machines).Id : Pick(random, production.WorkCenters).Id;
        var total = 480;
        var reserved = random.Next(120, 460);
        return new ResourceCapacitySnapshot
        {
            ResourceId = resourceId,
            ResourceType = isMachine ? ResourceType.Machine : ResourceType.WorkCenter,
            SnapshotDateUtc = DateTime.UtcNow.Date.AddDays(random.Next(-7, 14)),
            ShiftId = shift.Id,
            TotalCapacityMinutes = total,
            ReservedMinutes = reserved,
            AvailableMinutes = Math.Max(0, total - reserved),
            OvertimeMinutes = Math.Max(0, reserved - total),
            IsOverloaded = reserved > total,
            IsBottleneck = reserved > 420,
            CreatedBy = seedUser
        };
    }).ToList();

    await context.ScheduleOperationDependencies.AddRangeAsync(scheduleDependencies);
    await context.ScheduleOperationConstraints.AddRangeAsync(scheduleConstraints);
    await context.ScheduleOperationResourceOptions.AddRangeAsync(scheduleResourceOptions);
    await context.ScheduleResourceAssignments.AddRangeAsync(scheduleResourceAssignments);
    await context.CapacityReservations.AddRangeAsync(capacityReservations);
    await context.DispatchQueueItems.AddRangeAsync(dispatchQueue);
    await context.ScheduleMaterialChecks.AddRangeAsync(materialChecks);
    await context.ScheduleExceptions.AddRangeAsync(scheduleExceptions);
    await context.ScheduleRescheduleHistories.AddRangeAsync(scheduleRescheduleHistories);
    await context.ScheduleStatusHistories.AddRangeAsync(scheduleStatusHistories);
    await context.ScheduleAuditLogs.AddRangeAsync(scheduleAuditLogs);
    await context.ResourceCapacitySnapshots.AddRangeAsync(resourceCapacitySnapshots);
    await context.SaveChangesAsync();

    Console.WriteLine($"Seeded Scheduling: Plans={schedulePlans.Count}, Jobs={scheduleJobs.Count}, Operations={scheduleOperations.Count}");

    return new SchedulingSeedResult(schedulePlans, scheduleJobs, scheduleOperations);
}

static async Task<ShipmentSeedResult> SeedShipmentsAsync(
    OperationIntelligenceDbContext context,
    Random random,
    string runId,
    string seedUser,
    int targetCount,
    IReadOnlyList<Product> products,
    IReadOnlyList<Warehouse> warehouses,
    IReadOnlyList<UnitOfMeasure> uoms,
    IReadOnlyList<InventoryStock> stocks,
    IReadOnlyList<Order> orders,
    IReadOnlyList<OrderItem> orderItems,
    IReadOnlyList<ProductionOrder> productionOrders)
{
    var carrierCount = Math.Clamp(targetCount, 50, 120);
    var baseCarriers = new[]
    {
        ("PURO", "Purolator", "https://www.purolator.com"),
        ("FDX", "FedEx", "https://www.fedex.com"),
        ("UPS", "UPS", "https://www.ups.com"),
        ("DHL", "DHL", "https://www.dhl.com"),
        ("CPC", "Canada Post", "https://www.canadapost-postescanada.ca"),
        ("GLS", "GLS", "https://www.gls-group.com"),
        ("INTL", "Intelcom", "https://intelcom.ca"),
        ("LOOM", "Loomis Express", "https://www.loomisexpress.com")
    };
    var carriers = Enumerable.Range(1, carrierCount).Select(i =>
    {
        var baseCarrier = baseCarriers[(i - 1) % baseCarriers.Length];
        var code = carrierCount <= baseCarriers.Length ? baseCarrier.Item1 : $"{baseCarrier.Item1}{i:00}";
        var name = carrierCount <= baseCarriers.Length ? baseCarrier.Item2 : $"{baseCarrier.Item2} Regional {i:00}";
        return new Carrier
        {
            CarrierCode = code,
            Name = name,
            ContactName = GetPersonDirectory()[(i + 7) % GetPersonDirectory().Count],
            Phone = $"+1-647-{(4000 + i):0000}",
            Email = $"ops.{code.ToLowerInvariant()}@carrier-network.com",
            Website = baseCarrier.Item3,
            IsActive = true,
            CreatedBy = seedUser
        };
    }).ToList();

    await context.Carriers.AddRangeAsync(carriers);
    await context.SaveChangesAsync();

    var carrierServices = new List<CarrierService>(Math.Clamp(targetCount * 2, 100, 200));
    foreach (var carrier in carriers)
    {
        for (var s = 1; s <= 2; s++)
        {
            carrierServices.Add(new CarrierService
            {
                CarrierId = carrier.Id,
                ServiceCode = $"SVC-{s:00}",
                Name = s == 1 ? "Standard" : "Express",
                Description = "Seed carrier service",
                EstimatedTransitDays = s == 1 ? random.Next(3, 8) : random.Next(1, 3),
                IsActive = true,
                CreatedBy = seedUser
            });

            if (carrierServices.Count >= 200) break;
        }

        if (carrierServices.Count >= 200) break;
    }

    var shipmentAddressCount = Math.Clamp(targetCount * 2, 100, 200);
    var cityDirectory = GetCityDirectory();
    var addresses = Enumerable.Range(1, shipmentAddressCount).Select(i => new ShipmentAddress
    {
        AddressType = i % 2 == 0 ? "Shipping" : "Origin",
        ContactName = GetPersonDirectory()[(i + 3) % GetPersonDirectory().Count],
        CompanyName = i % 2 == 0 ? "NorthStar Retail Group" : "Ops Intelligence Logistics",
        Phone = $"+1-905-{(1000 + i):0000}",
        Email = $"shipping.contact.{runId}.{i}@opsmail.com",
        AddressLine1 = $"{100 + i} {Pick(random, GetStreetNames())}",
        AddressLine2 = "Unit 1",
        City = cityDirectory[i % cityDirectory.Count].City,
        StateOrProvince = cityDirectory[i % cityDirectory.Count].Province,
        PostalCode = BuildCanadianPostalCode(random),
        Country = "Canada",
        CreatedBy = seedUser
    }).ToList();

    var deliveryRuns = Enumerable.Range(1, Math.Clamp(targetCount + 20, 70, 200)).Select(i =>
    {
        var wh = Pick(random, warehouses);
        var start = NextDate(random, DateTime.UtcNow.AddDays(-10), DateTime.UtcNow.AddDays(20));
        return new DeliveryRun
        {
            RunNumber = $"RUN-{runId}-{i:0000}",
            Name = $"Delivery Run {i}",
            WarehouseId = wh.Id,
            PlannedStartUtc = start,
            PlannedEndUtc = start.AddHours(random.Next(4, 12)),
            DriverName = $"Driver {i}",
            VehicleNumber = $"TRK-{100 + i}",
            RouteCode = $"R-{i:000}",
            Status = RandomEnum<DeliveryRunStatus>(random),
            Notes = "Seed delivery run",
            CreatedBy = seedUser
        };
    }).ToList();

    var dockAppointments = Enumerable.Range(1, Math.Clamp(targetCount * 2, 100, 200)).Select(i =>
    {
        var wh = Pick(random, warehouses);
        var carrier = random.Next(0, 10) < 8 ? Pick(random, carriers) : null;
        var start = NextDate(random, DateTime.UtcNow.AddDays(-5), DateTime.UtcNow.AddDays(20));
        return new DockAppointment
        {
            AppointmentNumber = $"APT-{runId}-{i:0000}",
            WarehouseId = wh.Id,
            CarrierId = carrier?.Id,
            DockCode = $"D{(i % 20) + 1:00}",
            TrailerNumber = $"TRL-{200 + i}",
            DriverName = $"DockDriver {i}",
            ScheduledStartUtc = start,
            ScheduledEndUtc = start.AddHours(2),
            Status = RandomEnum<DockAppointmentStatus>(random),
            Notes = "Seed dock appointment",
            CreatedBy = seedUser
        };
    }).ToList();

    await context.CarrierServices.AddRangeAsync(carrierServices);
    await context.ShipmentAddresses.AddRangeAsync(addresses);
    await context.DeliveryRuns.AddRangeAsync(deliveryRuns);
    await context.DockAppointments.AddRangeAsync(dockAppointments);
    await context.SaveChangesAsync();

    var runsByWarehouse = deliveryRuns.GroupBy(x => x.WarehouseId).ToDictionary(g => g.Key, g => g.ToList());
    var docksByWarehouse = dockAppointments.GroupBy(x => x.WarehouseId).ToDictionary(g => g.Key, g => g.ToList());
    var carrierServicesByCarrier = carrierServices.GroupBy(x => x.CarrierId).ToDictionary(g => g.Key, g => g.ToList());

    var shipmentCount = Math.Clamp(targetCount * 2 + 30, 130, 200);
    var shipments = new List<Shipment>(shipmentCount);

    for (var i = 1; i <= shipmentCount; i++)
    {
        var order = random.Next(0, 10) < 8 && orders.Count > 0 ? Pick(random, orders) : null;
        var warehouse = order?.WarehouseId.HasValue == true
            ? warehouses.FirstOrDefault(x => x.Id == order.WarehouseId.Value) ?? Pick(random, warehouses)
            : Pick(random, warehouses);

        var carrier = random.Next(0, 10) < 8 ? Pick(random, carriers) : null;
        var service = carrier != null && carrierServicesByCarrier.TryGetValue(carrier.Id, out var svcList)
            ? Pick(random, svcList)
            : null;

        var origin = Pick(random, addresses);
        var destination = Pick(random, addresses);
        if (destination.Id == origin.Id)
        {
            destination = addresses[(addresses.IndexOf(origin) + 1) % addresses.Count];
        }

        var plannedShip = NextDate(random, DateTime.UtcNow.AddDays(-15), DateTime.UtcNow.AddDays(15));
        var isCrossBorder = random.Next(0, 5) == 0;

        runsByWarehouse.TryGetValue(warehouse.Id, out var whRuns);
        docksByWarehouse.TryGetValue(warehouse.Id, out var whDocks);

        shipments.Add(new Shipment
        {
            ShipmentNumber = $"SHP-{runId}-{i:00000}",
            OrderId = order?.Id,
            WarehouseId = warehouse.Id,
            CarrierId = carrier?.Id,
            CarrierServiceId = service?.Id,
            OriginAddressId = origin.Id,
            DestinationAddressId = destination.Id,
            DeliveryRunId = whRuns != null && whRuns.Count > 0 && random.Next(0, 2) == 0 ? Pick(random, whRuns).Id : null,
            DockAppointmentId = whDocks != null && whDocks.Count > 0 && random.Next(0, 3) == 0 ? Pick(random, whDocks).Id : null,
            Type = RandomEnum<ShipmentType>(random),
            Status = RandomEnum<ShipmentStatus>(random),
            Priority = RandomEnum<ShipmentPriority>(random),
            CustomerReference = order?.OrderNumber,
            ExternalReference = $"EXT-{runId}-{i:0000}",
            TrackingNumber = $"TRK-{runId}-{i:000000}",
            MasterTrackingNumber = $"MTRK-{runId}-{i:000000}",
            PlannedShipDateUtc = plannedShip,
            PlannedDeliveryDateUtc = plannedShip.AddDays(random.Next(1, 8)),
            ActualShipDateUtc = random.Next(0, 2) == 0 ? plannedShip.AddHours(2) : null,
            ActualDeliveryDateUtc = null,
            ScheduledPickupStartUtc = plannedShip.AddHours(-4),
            ScheduledPickupEndUtc = plannedShip.AddHours(-2),
            CurrencyCode = "CAD",
            ShippingTerms = "DAP",
            Incoterm = "DAP",
            IsPartialShipment = random.Next(0, 2) == 0,
            RequiresSignature = random.Next(0, 3) == 0,
            IsFragile = random.Next(0, 5) == 0,
            IsHazardous = random.Next(0, 12) == 0,
            IsTemperatureControlled = random.Next(0, 8) == 0,
            IsInsured = random.Next(0, 2) == 0,
            IsCrossBorder = isCrossBorder,
            Notes = "Seed shipment",
            InternalNotes = "Internal seeded note",
            CreatedBy = seedUser
        });
    }

    await context.Shipments.AddRangeAsync(shipments);
    await context.SaveChangesAsync();

    var orderItemsByOrder = orderItems.GroupBy(x => x.OrderId).ToDictionary(g => g.Key, g => g.ToList());
    var shipmentItems = new List<ShipmentItem>(Math.Clamp(targetCount * 3, 150, 200));

    foreach (var shipment in shipments)
    {
        var lines = random.Next(1, 3);
        for (var line = 1; line <= lines; line++)
        {
            if (shipmentItems.Count >= 200) break;

            var maybeOrderItems = shipment.OrderId.HasValue && orderItemsByOrder.TryGetValue(shipment.OrderId.Value, out var oi)
                ? oi
                : null;

            var orderItem = maybeOrderItems != null && maybeOrderItems.Count > 0 ? Pick(random, maybeOrderItems) : null;
            var product = orderItem != null
                ? products.First(x => x.Id == orderItem.ProductId)
                : Pick(random, products);

            var orderedQty = orderItem?.QuantityOrdered ?? NextDecimal(random, 1m, 25m);
            var packedQty = NextDecimal(random, 1m, orderedQty);
            var shippedQty = NextDecimal(random, 0m, packedQty);
            var deliveredQty = NextDecimal(random, 0m, shippedQty);

            var stock = stocks.FirstOrDefault(x => x.ProductId == product.Id && x.WarehouseId == shipment.WarehouseId);

            shipmentItems.Add(new ShipmentItem
            {
                ShipmentId = shipment.Id,
                OrderItemId = orderItem?.Id,
                ProductId = product.Id,
                WarehouseId = shipment.WarehouseId,
                UnitOfMeasureId = product.UnitOfMeasureId == Guid.Empty ? Pick(random, uoms).Id : product.UnitOfMeasureId,
                InventoryStockId = stock?.Id,
                ProductionOrderId = random.Next(0, 3) == 0 && productionOrders.Count > 0 ? Pick(random, productionOrders).Id : null,
                LineNumber = line.ToString("000"),
                OrderedQuantity = orderedQty,
                AllocatedQuantity = orderedQty,
                PickedQuantity = packedQty,
                PackedQuantity = packedQty,
                ShippedQuantity = shippedQty,
                DeliveredQuantity = deliveredQty,
                ReturnedQuantity = 0m,
                UnitWeight = product.Weight > 0 ? product.Weight : NextDecimal(random, 0.2m, 6m),
                UnitVolume = product.Length > 0 && product.Width > 0 && product.Height > 0
                    ? product.Length * product.Width * product.Height
                    : NextDecimal(random, 0.01m, 0.8m),
                LotNumber = $"LOT-{runId}-{shipmentItems.Count + 1:0000}",
                SerialNumber = product.IsSerialized ? $"SER-{runId}-{shipmentItems.Count + 1:000000}" : null,
                ExpiryDateUtc = product.IsPerishable ? DateTime.UtcNow.AddMonths(random.Next(3, 18)) : null,
                Status = RandomEnum<ShipmentItemStatus>(random),
                Notes = "Seed shipment item",
                CreatedBy = seedUser
            });
        }

        if (shipmentItems.Count >= 200) break;
    }

    await context.ShipmentItems.AddRangeAsync(shipmentItems);
    await context.SaveChangesAsync();

    var itemsByShipment = shipmentItems.GroupBy(x => x.ShipmentId).ToDictionary(g => g.Key, g => g.ToList());

    var shipmentPackages = new List<ShipmentPackage>(Math.Clamp(targetCount * 3, 150, 200));
    foreach (var shipment in shipments)
    {
        var pkgCount = random.Next(1, 3);
        for (var p = 1; p <= pkgCount; p++)
        {
            if (shipmentPackages.Count >= 200) break;
            shipmentPackages.Add(new ShipmentPackage
            {
                ShipmentId = shipment.Id,
                PackageNumber = p.ToString("000"),
                TrackingNumber = $"PKGTRK-{runId}-{shipmentPackages.Count + 1:000000}",
                PackageType = random.Next(0, 2) == 0 ? "Box" : "Pallet",
                Length = NextDecimal(random, 10m, 120m),
                Width = NextDecimal(random, 10m, 100m),
                Height = NextDecimal(random, 5m, 90m),
                Weight = NextDecimal(random, 1m, 70m),
                DeclaredValue = NextDecimal(random, 50m, 3000m),
                RequiresSpecialHandling = random.Next(0, 5) == 0,
                IsFragile = random.Next(0, 4) == 0,
                LabelUrl = "https://cdn.seed.local/label.pdf",
                Barcode = $"BC-{runId}-{shipmentPackages.Count + 1:000000}",
                Status = RandomEnum<PackageStatus>(random),
                CreatedBy = seedUser
            });
        }

        if (shipmentPackages.Count >= 200) break;
    }

    await context.ShipmentPackages.AddRangeAsync(shipmentPackages);
    await context.SaveChangesAsync();

    var packagesByShipment = shipmentPackages.GroupBy(x => x.ShipmentId).ToDictionary(g => g.Key, g => g.ToList());

    var shipmentPackageItems = new List<ShipmentPackageItem>(Math.Clamp(targetCount * 3, 150, 200));
    foreach (var shipment in shipments)
    {
        if (!itemsByShipment.TryGetValue(shipment.Id, out var items) || items.Count == 0) continue;
        if (!packagesByShipment.TryGetValue(shipment.Id, out var packages) || packages.Count == 0) continue;

        foreach (var pkg in packages)
        {
            if (shipmentPackageItems.Count >= 200) break;
            var item = Pick(random, items);
            if (shipmentPackageItems.Any(x => x.ShipmentPackageId == pkg.Id && x.ShipmentItemId == item.Id))
            {
                continue;
            }

            shipmentPackageItems.Add(new ShipmentPackageItem
            {
                ShipmentPackageId = pkg.Id,
                ShipmentItemId = item.Id,
                Quantity = Math.Min(item.PackedQuantity <= 0 ? 1m : item.PackedQuantity, NextDecimal(random, 1m, 5m)),
                CreatedBy = seedUser
            });
        }

        if (shipmentPackageItems.Count >= 200) break;
    }

    await context.ShipmentPackageItems.AddRangeAsync(shipmentPackageItems);

    var shipmentTrackingEvents = shipments.Take(Math.Clamp(targetCount * 3, 150, 200)).Select((s, i) => new ShipmentTrackingEvent
    {
        ShipmentId = s.Id,
        EventCode = i % 2 == 0 ? "PICKED_UP" : "IN_TRANSIT",
        EventName = i % 2 == 0 ? "Picked Up" : "In Transit",
        Description = "Seed tracking event",
        EventTimeUtc = (s.PlannedShipDateUtc ?? DateTime.UtcNow).AddHours(i % 6),
        LocationName = "Seed Hub",
        City = "Toronto",
        StateOrProvince = "ON",
        Country = "Canada",
        CarrierStatusCode = "OK",
        Source = "Seeder",
        IsCustomerVisible = true,
        CreatedBy = seedUser
    }).ToList();

    var shipmentDocuments = shipments.Take(Math.Clamp(targetCount * 2, 100, 200)).Select((s, i) => new ShipmentDocument
    {
        ShipmentId = s.Id,
        DocumentType = RandomEnum<ShipmentDocumentType>(random),
        FileName = $"shipment-{s.ShipmentNumber}.pdf",
        FileUrl = $"https://cdn.seed.local/shipments/{s.ShipmentNumber}.pdf",
        ContentType = "application/pdf",
        FileSizeBytes = random.Next(40_000, 450_000),
        IsCustomerVisible = random.Next(0, 2) == 0,
        Notes = "Seed document",
        CreatedBy = seedUser
    }).ToList();

    var shipmentCharges = shipments.Take(Math.Clamp(targetCount * 3, 150, 200)).Select((s, i) => new ShipmentCharge
    {
        ShipmentId = s.Id,
        ChargeType = RandomEnum<ShipmentChargeType>(random),
        Description = "Seed charge",
        Amount = NextDecimal(random, 5m, 200m),
        CurrencyCode = "CAD",
        CreatedBy = seedUser
    }).ToList();

    var shipmentExceptions = shipments.Take(Math.Clamp(targetCount * 2, 100, 200)).Select((s, i) => new ShipmentException
    {
        ShipmentId = s.Id,
        ExceptionType = RandomEnum<ShipmentExceptionType>(random),
        Title = "Seed exception",
        Description = "Seed exception detail",
        ReportedAtUtc = DateTime.UtcNow.AddDays(-random.Next(0, 10)),
        ReportedBy = "seed-system",
        IsResolved = random.Next(0, 2) == 0,
        ResolvedAtUtc = random.Next(0, 2) == 0 ? DateTime.UtcNow : null,
        ResolutionNote = "Seed resolution",
        CreatedBy = seedUser
    }).ToList();

    var insuredShipments = shipments.Where(x => x.IsInsured).Take(Math.Clamp(targetCount * 2, 80, 200)).ToList();
    var shipmentInsurances = insuredShipments.Select((s, i) => new ShipmentInsurance
    {
        ShipmentId = s.Id,
        ProviderName = "Seed Insurance",
        PolicyNumber = $"POL-{runId}-{i:00000}",
        InsuredAmount = NextDecimal(random, 500m, 10_000m),
        PremiumAmount = NextDecimal(random, 20m, 500m),
        CurrencyCode = "CAD",
        EffectiveDateUtc = s.PlannedShipDateUtc ?? DateTime.UtcNow,
        ExpiryDateUtc = (s.PlannedDeliveryDateUtc ?? DateTime.UtcNow).AddDays(30),
        Status = InsuranceStatus.Active,
        Notes = "Seed insurance",
        CreatedBy = seedUser
    }).ToList();

    var shipmentStatusHistory = shipments.Take(Math.Clamp(targetCount * 3, 150, 200)).Select((s, i) => new ShipmentStatusHistory
    {
        ShipmentId = s.Id,
        FromStatus = ShipmentStatus.Draft,
        ToStatus = s.Status,
        ChangedAtUtc = s.CreatedAtUtc,
        ChangedBy = "seed-system",
        Reason = "Initial seeded status",
        CreatedBy = seedUser
    }).ToList();

    var crossBorderShipments = shipments.Where(x => x.IsCrossBorder).Take(Math.Clamp(targetCount * 2, 80, 200)).ToList();
    var customsDocuments = crossBorderShipments.Select((s, i) => new CustomsDocument
    {
        ShipmentId = s.Id,
        DocumentType = RandomEnum<CustomsDocumentType>(random),
        DocumentNumber = $"CUST-{runId}-{i:00000}",
        FileName = $"customs-{s.ShipmentNumber}.pdf",
        FileUrl = $"https://cdn.seed.local/customs/{s.ShipmentNumber}.pdf",
        CountryOfOrigin = "Canada",
        DestinationCountry = "United States",
        HarmonizedCode = $"{random.Next(1000, 9999)}.{random.Next(10, 99)}",
        DeclaredCustomsValue = NextDecimal(random, 100m, 5000m),
        CurrencyCode = "CAD",
        IssuedAtUtc = DateTime.UtcNow.AddDays(-random.Next(0, 15)),
        Notes = "Seed customs doc",
        CreatedBy = seedUser
    }).ToList();

    await context.ShipmentTrackingEvents.AddRangeAsync(shipmentTrackingEvents);
    await context.ShipmentDocuments.AddRangeAsync(shipmentDocuments);
    await context.ShipmentCharges.AddRangeAsync(shipmentCharges);
    await context.ShipmentExceptions.AddRangeAsync(shipmentExceptions);
    await context.ShipmentInsurances.AddRangeAsync(shipmentInsurances);
    await context.ShipmentStatusHistories.AddRangeAsync(shipmentStatusHistory);
    await context.CustomsDocuments.AddRangeAsync(customsDocuments);
    await context.SaveChangesAsync();

    var returnShipments = new List<ReturnShipment>(Math.Clamp(targetCount * 2, 80, 200));
    var returnedShipmentPool = shipments.Where(x => x.Status == ShipmentStatus.Delivered || x.Status == ShipmentStatus.Returned).ToList();
    if (returnedShipmentPool.Count == 0)
    {
        returnedShipmentPool = shipments.Take(100).ToList();
    }

    for (var i = 1; i <= Math.Clamp(targetCount * 2, 80, 200); i++)
    {
        var shipment = Pick(random, returnedShipmentPool);
        var origin = Pick(random, addresses);
        var dest = Pick(random, addresses);
        var carrier = shipment.CarrierId.HasValue ? carriers.FirstOrDefault(x => x.Id == shipment.CarrierId.Value) : Pick(random, carriers);

        returnShipments.Add(new ReturnShipment
        {
            ReturnShipmentNumber = $"RMA-{runId}-{i:00000}",
            ShipmentId = shipment.Id,
            OrderId = shipment.OrderId,
            OriginAddressId = origin.Id,
            DestinationAddressId = dest.Id,
            CarrierId = carrier?.Id,
            CarrierServiceId = carrier != null && carrierServicesByCarrier.TryGetValue(carrier.Id, out var svcs) && svcs.Count > 0
                ? Pick(random, svcs).Id
                : null,
            TrackingNumber = $"RTRK-{runId}-{i:00000}",
            ReasonCode = "DAMAGED",
            ReasonDescription = "Seeded return",
            Status = RandomEnum<ReturnShipmentStatus>(random),
            RequestedAtUtc = DateTime.UtcNow.AddDays(-random.Next(0, 20)),
            ReceivedAtUtc = random.Next(0, 2) == 0 ? DateTime.UtcNow.AddDays(-random.Next(0, 5)) : null,
            Notes = "Seed return shipment",
            CreatedBy = seedUser
        });
    }

    await context.ReturnShipments.AddRangeAsync(returnShipments);
    await context.SaveChangesAsync();

    var shipmentItemsByShipment = shipmentItems.GroupBy(x => x.ShipmentId).ToDictionary(g => g.Key, g => g.ToList());
    var returnItems = new List<ReturnShipmentItem>(Math.Clamp(targetCount * 2, 80, 200));

    foreach (var rs in returnShipments)
    {
        if (!shipmentItemsByShipment.TryGetValue(rs.ShipmentId, out var items) || items.Count == 0) continue;

        var item = Pick(random, items);
        if (returnItems.Any(x => x.ReturnShipmentId == rs.Id && x.ShipmentItemId == item.Id))
        {
            continue;
        }

        returnItems.Add(new ReturnShipmentItem
        {
            ReturnShipmentId = rs.Id,
            ShipmentItemId = item.Id,
            ReturnedQuantity = Math.Max(1m, Math.Min(item.DeliveredQuantity > 0 ? item.DeliveredQuantity : item.ShippedQuantity, NextDecimal(random, 1m, 5m))),
            ReturnCondition = random.Next(0, 2) == 0 ? "Opened" : "Damaged",
            InspectionResult = "Seeded inspection",
            Notes = "Seed return item",
            CreatedBy = seedUser
        });

        if (returnItems.Count >= 200) break;
    }

    await context.ReturnShipmentItems.AddRangeAsync(returnItems);

    // Recalculate shipment rollups to keep consistency
    var packageByShipment = shipmentPackages.GroupBy(x => x.ShipmentId).ToDictionary(g => g.Key, g => g.ToList());
    var chargesByShipment = shipmentCharges.GroupBy(x => x.ShipmentId).ToDictionary(g => g.Key, g => g.ToList());

    foreach (var shipment in shipments)
    {
        var packages = packageByShipment.TryGetValue(shipment.Id, out var p) ? p : new List<ShipmentPackage>();
        var charges = chargesByShipment.TryGetValue(shipment.Id, out var c) ? c : new List<ShipmentCharge>();

        shipment.TotalPackages = packages.Count;
        shipment.TotalWeight = packages.Sum(x => x.Weight);
        shipment.TotalVolume = packages.Sum(x => x.Length * x.Width * x.Height);
        shipment.FreightCost = charges.Where(x => x.ChargeType == ShipmentChargeType.Freight).Sum(x => x.Amount);
        shipment.InsuranceCost = charges.Where(x => x.ChargeType == ShipmentChargeType.Insurance).Sum(x => x.Amount);
        shipment.OtherCharges = charges.Where(x => x.ChargeType != ShipmentChargeType.Freight && x.ChargeType != ShipmentChargeType.Insurance).Sum(x => x.Amount);
        shipment.TotalShippingCost = charges.Sum(x => x.Amount);
    }

    context.Shipments.UpdateRange(shipments);
    await context.SaveChangesAsync();

    Console.WriteLine($"Seeded Shipments: Carriers={carriers.Count}, Shipments={shipments.Count}, ReturnShipments={returnShipments.Count}");

    return new ShipmentSeedResult(shipments, shipmentItems, returnShipments);
}

internal sealed record ProductionSeedResult(
    List<WorkCenter> WorkCenters,
    List<Machine> Machines,
    List<Routing> Routings,
    List<RoutingStep> RoutingSteps,
    List<BillOfMaterial> BillOfMaterials,
    List<ProductionOrder> ProductionOrders,
    List<ProductionExecution> ProductionExecutions);

internal sealed record SchedulingSeedResult(
    List<SchedulePlan> SchedulePlans,
    List<ScheduleJob> ScheduleJobs,
    List<ScheduleOperation> ScheduleOperations);

internal sealed record ShipmentSeedResult(
    List<Shipment> Shipments,
    List<ShipmentItem> ShipmentItems,
    List<ReturnShipment> ReturnShipments);
