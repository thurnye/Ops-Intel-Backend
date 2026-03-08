using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class InventoryStockConfiguration : IEntityTypeConfiguration<InventoryStock>
{
    public void Configure(EntityTypeBuilder<InventoryStock> builder)
    {
        builder.ToTable("InventoryStocks");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.QuantityOnHand).HasColumnType("decimal(18,2)");
        builder.Property(x => x.QuantityReserved).HasColumnType("decimal(18,2)");
        builder.Property(x => x.QuantityAvailable).HasColumnType("decimal(18,2)");
        builder.Property(x => x.QuantityDamaged).HasColumnType("decimal(18,2)");

        builder.HasIndex(x => new { x.ProductId, x.WarehouseId }).IsUnique();

        builder.HasOne(x => x.Product)
            .WithMany(x => x.InventoryStocks)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Warehouse)
            .WithMany(x => x.InventoryStocks)
            .HasForeignKey(x => x.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}