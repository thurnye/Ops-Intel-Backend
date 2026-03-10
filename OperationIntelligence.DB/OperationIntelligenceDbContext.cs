using Microsoft.EntityFrameworkCore;


namespace OperationIntelligence.DB
{
    public class OperationIntelligenceDbContext : DbContext
    {
        public OperationIntelligenceDbContext(DbContextOptions<OperationIntelligenceDbContext> options)
            : base(options)
        {
        }
        // =========================
        // Auth DbSets
        // =========================
        public DbSet<PlatformUser> Users => Set<PlatformUser>();
        public DbSet<PlatformUserProfile> UserProfiles => Set<PlatformUserProfile>();
        public DbSet<MediaFile> MediaFiles => Set<MediaFile>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Permission> Permissions => Set<Permission>();
        public DbSet<UserRole> UserRoles => Set<UserRole>();
        public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<UserSession> UserSessions => Set<UserSession>();
        public DbSet<EmailVerificationToken> EmailVerificationTokens => Set<EmailVerificationToken>();
        public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();
        public DbSet<PasswordHistory> PasswordHistories => Set<PasswordHistory>();
        public DbSet<LoginAttempt> LoginAttempts => Set<LoginAttempt>();

        // =========================
        // Inventory DbSets
        // =========================
        public DbSet<Product> Products => Set<Product>();
        public DbSet<ProductImage> ProductImages => Set<ProductImage>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Brand> Brands => Set<Brand>();
        public DbSet<UnitOfMeasure> UnitsOfMeasure => Set<UnitOfMeasure>();
        public DbSet<Warehouse> Warehouses => Set<Warehouse>();
        public DbSet<InventoryStock> InventoryStocks => Set<InventoryStock>();
        public DbSet<StockMovement> StockMovements => Set<StockMovement>();
        public DbSet<Supplier> Suppliers => Set<Supplier>();
        public DbSet<ProductSupplier> ProductSuppliers => Set<ProductSupplier>();


        // =========================
        // Order DbSets
        // =========================
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<OrderImage> OrderImages => Set<OrderImage>();
        public DbSet<OrderAddress> OrderAddresses => Set<OrderAddress>();
        public DbSet<OrderNote> OrderNotes => Set<OrderNote>();
        public DbSet<OrderStatusHistory> OrderStatusHistories => Set<OrderStatusHistory>();
        public DbSet<OrderPayment> OrderPayments => Set<OrderPayment>();


        // =========================
        // Production DbSets
        // =========================

        public DbSet<WorkCenter> WorkCenters => Set<WorkCenter>();
        public DbSet<Machine> Machines => Set<Machine>();
        public DbSet<BillOfMaterial> BillsOfMaterial => Set<BillOfMaterial>();
        public DbSet<BillOfMaterialItem> BillOfMaterialItems => Set<BillOfMaterialItem>();
        public DbSet<Routing> Routings => Set<Routing>();
        public DbSet<RoutingStep> RoutingSteps => Set<RoutingStep>();
        public DbSet<ProductionOrder> ProductionOrders => Set<ProductionOrder>();
        public DbSet<ProductionExecution> ProductionExecutions => Set<ProductionExecution>();
        public DbSet<ProductionMaterialIssue> ProductionMaterialIssues => Set<ProductionMaterialIssue>();
        public DbSet<ProductionMaterialConsumption> ProductionMaterialConsumptions => Set<ProductionMaterialConsumption>();
        public DbSet<ProductionOutput> ProductionOutputs => Set<ProductionOutput>();
        public DbSet<ProductionScrap> ProductionScraps => Set<ProductionScrap>();
        public DbSet<ProductionDowntime> ProductionDowntimes => Set<ProductionDowntime>();
        public DbSet<ProductionLaborLog> ProductionLaborLogs => Set<ProductionLaborLog>();
        public DbSet<ProductionQualityCheck> ProductionQualityChecks => Set<ProductionQualityCheck>();


        // =========================
        // Scheduling DbSets
        // =========================

        public DbSet<SchedulePlan> SchedulePlans { get; set; }
        public DbSet<ScheduleJob> ScheduleJobs { get; set; }
        public DbSet<ScheduleOperation> ScheduleOperations { get; set; }
        public DbSet<ScheduleOperationDependency> ScheduleOperationDependencies { get; set; }
        public DbSet<ScheduleOperationConstraint> ScheduleOperationConstraints { get; set; }
        public DbSet<ScheduleOperationResourceOption> ScheduleOperationResourceOptions { get; set; }
        public DbSet<ScheduleResourceAssignment> ScheduleResourceAssignments { get; set; }
        public DbSet<Shift> Shifts { get; set; }
        public DbSet<ResourceCalendar> ResourceCalendars { get; set; }
        public DbSet<ResourceCalendarException> ResourceCalendarExceptions { get; set; }
        public DbSet<CapacityReservation> CapacityReservations { get; set; }
        public DbSet<ResourceCapacitySnapshot> ResourceCapacitySnapshots { get; set; }
        public DbSet<DispatchQueueItem> DispatchQueueItems { get; set; }
        public DbSet<ScheduleMaterialCheck> ScheduleMaterialChecks { get; set; }
        public DbSet<ScheduleException> ScheduleExceptions { get; set; }
        public DbSet<ScheduleRevision> ScheduleRevisions { get; set; }
        public DbSet<ScheduleRescheduleHistory> ScheduleRescheduleHistories { get; set; }
        public DbSet<ScheduleStatusHistory> ScheduleStatusHistories { get; set; }
        public DbSet<ScheduleAuditLog> ScheduleAuditLogs { get; set; }

        // =========================
        // Shipments DbSets
        // =========================
        public DbSet<Shipment> Shipments => Set<Shipment>();
        public DbSet<ShipmentItem> ShipmentItems => Set<ShipmentItem>();
        public DbSet<ShipmentPackage> ShipmentPackages => Set<ShipmentPackage>();
        public DbSet<ShipmentPackageItem> ShipmentPackageItems => Set<ShipmentPackageItem>();
        public DbSet<ShipmentTrackingEvent> ShipmentTrackingEvents => Set<ShipmentTrackingEvent>();
        public DbSet<ShipmentDocument> ShipmentDocuments => Set<ShipmentDocument>();
        public DbSet<ShipmentStatusHistory> ShipmentStatusHistories => Set<ShipmentStatusHistory>();
        public DbSet<ShipmentException> ShipmentExceptions => Set<ShipmentException>();
        public DbSet<ShipmentCharge> ShipmentCharges => Set<ShipmentCharge>();
        public DbSet<Carrier> Carriers => Set<Carrier>();
        public DbSet<CarrierService> CarrierServices => Set<CarrierService>();
        public DbSet<ShipmentAddress> ShipmentAddresses => Set<ShipmentAddress>();
        public DbSet<DeliveryRun> DeliveryRuns => Set<DeliveryRun>();
        public DbSet<DockAppointment> DockAppointments => Set<DockAppointment>();
        public DbSet<ShipmentInsurance> ShipmentInsurances => Set<ShipmentInsurance>();
        public DbSet<ReturnShipment> ReturnShipments => Set<ReturnShipment>();
        public DbSet<ReturnShipmentItem> ReturnShipmentItems => Set<ReturnShipmentItem>();
        public DbSet<CustomsDocument> CustomsDocuments => Set<CustomsDocument>();

        // protected override void OnModelCreating(ModelBuilder modelBuilder)
        // {
        //     // Auth
        //     modelBuilder.ApplyConfiguration(new PlatformUserConfiguration());
        //     modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());


        //     // Inventory
        //     modelBuilder.ApplyConfiguration(new ProductConfiguration());
        //     modelBuilder.ApplyConfiguration(new ProductImageConfiguration());
        //     modelBuilder.ApplyConfiguration(new InventoryStockConfiguration());
        //     modelBuilder.ApplyConfiguration(new StockMovementConfiguration());
        //     modelBuilder.ApplyConfiguration(new CategoryConfiguration());
        //     modelBuilder.ApplyConfiguration(new BrandConfiguration());
        //     modelBuilder.ApplyConfiguration(new UnitOfMeasureConfiguration());
        //     modelBuilder.ApplyConfiguration(new WarehouseConfiguration());
        //     modelBuilder.ApplyConfiguration(new SupplierConfiguration());
        //     modelBuilder.ApplyConfiguration(new ProductSupplierConfiguration());

        //     // Order
        //     modelBuilder.ApplyConfiguration(new OrderConfiguration());
        //     modelBuilder.ApplyConfiguration(new OrderImageConfiguration());
        //     modelBuilder.ApplyConfiguration(new OrderItemConfiguration());
        //     modelBuilder.ApplyConfiguration(new OrderPaymentConfiguration());
        //     modelBuilder.ApplyConfiguration(new OrderAddressConfiguration());
        //     modelBuilder.ApplyConfiguration(new OrderNoteConfiguration());
        //     modelBuilder.ApplyConfiguration(new OrderStatusHistoryConfiguration());


        //     ConfigureGlobalConventions(modelBuilder);
        // }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(OperationIntelligenceDbContext).Assembly);

            ConfigureGlobalConventions(modelBuilder);
        }

        private static void ConfigureGlobalConventions(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(decimal) || property.ClrType == typeof(decimal?))
                    {
                        property.SetPrecision(18);
                        property.SetScale(2);
                    }

                    if (property.ClrType == typeof(DateTime))
                    {
                        // Optional: enforce datetime2 in SQL Server
                        property.SetColumnType("datetime2");
                    }

                    if (property.ClrType == typeof(decimal) || property.ClrType == typeof(decimal?))
                    {
                        if (property.Name.Contains("Quantity"))
                        {
                            property.SetPrecision(18);
                            property.SetScale(4);
                        }
                        else
                        {
                            property.SetPrecision(18);
                            property.SetScale(2);
                        }
                    }
                }
            }
        }

        public override int SaveChanges()
        {
            ApplyAuditInformation();
            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            ApplyAuditInformation();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyAuditInformation();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            ApplyAuditInformation();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void ApplyAuditInformation()
        {
            var utcNow = DateTime.UtcNow;

            var entries = ChangeTracker.Entries<AuditableEntity>();

            foreach (var entry in entries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAtUtc = utcNow;
                        entry.Entity.UpdatedAtUtc = null;
                        entry.Entity.DeletedAtUtc = null;
                        entry.Entity.IsDeleted = false;
                        break;

                    case EntityState.Modified:
                        entry.Entity.UpdatedAtUtc = utcNow;

                        // Prevent accidental overwrite of created audit fields
                        entry.Property(x => x.CreatedAtUtc).IsModified = false;
                        entry.Property(x => x.CreatedBy).IsModified = false;
                        break;

                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;
                        entry.Entity.IsDeleted = true;
                        entry.Entity.DeletedAtUtc = utcNow;
                        entry.Entity.UpdatedAtUtc = utcNow;
                        break;
                }
            }
        }
    }
}
