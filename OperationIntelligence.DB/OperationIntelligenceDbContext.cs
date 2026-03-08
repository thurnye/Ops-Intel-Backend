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

        // Inventory
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


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            // modelBuilder.Entity<PlatformUser>(entity =>
            // {
            //     entity.ToTable("ApplicationUser");
            //     entity.HasKey(u => u.Id);
            //     entity.HasIndex(u => u.Email).IsUnique();
            // });

            // modelBuilder.Entity<RefreshToken>(entity =>
            // {
            //     entity.HasKey(rt => rt.Id);
            //     entity.Property(rt => rt.Token).IsRequired().HasMaxLength(256);
            //     entity.HasIndex(rt => rt.Token).IsUnique();

            //     Configure the relationship to PlatformUser
            //     entity.HasOne(rt => rt.User)
            //           .WithMany()
            //           .HasForeignKey(rt => rt.UserId)
            //           .OnDelete(DeleteBehavior.Cascade);
            // });

            // Auth
            modelBuilder.ApplyConfiguration(new PlatformUserConfiguration());
            modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());


            // Inventory
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
            modelBuilder.ApplyConfiguration(new ProductImageConfiguration());
            modelBuilder.ApplyConfiguration(new InventoryStockConfiguration());
            modelBuilder.ApplyConfiguration(new StockMovementConfiguration());
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            modelBuilder.ApplyConfiguration(new BrandConfiguration());
            modelBuilder.ApplyConfiguration(new UnitOfMeasureConfiguration());
            modelBuilder.ApplyConfiguration(new WarehouseConfiguration());
            modelBuilder.ApplyConfiguration(new SupplierConfiguration());
            modelBuilder.ApplyConfiguration(new ProductSupplierConfiguration());
        }
    }
}
