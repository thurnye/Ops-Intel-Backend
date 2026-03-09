using OperationIntelligence.DB.Entities;
using Microsoft.EntityFrameworkCore;


namespace OperationIntelligence.DB
{
    public class OperationIntelligenceDbContext : DbContext
    {
        public OperationIntelligenceDbContext(DbContextOptions<OperationIntelligenceDbContext> options)
            : base(options)
        {
        }
        public DbSet<PlatformUser> Users => Set<PlatformUser>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

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

                    if (property.ClrType == typeof(DateTime?))
                    {
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
    }
}
