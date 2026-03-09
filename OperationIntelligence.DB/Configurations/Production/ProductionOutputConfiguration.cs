using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class ProductionOutputConfiguration : IEntityTypeConfiguration<ProductionOutput>
{
    public void Configure(EntityTypeBuilder<ProductionOutput> builder)
    {
        builder.ToTable("ProductionOutputs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.QuantityProduced)
            .HasPrecision(18, 4);

        builder.Property(x => x.BatchNumber)
            .HasMaxLength(100);

        builder.Property(x => x.LotNumber)
            .HasMaxLength(100);

        builder.Property(x => x.IsFinalOutput)
            .HasDefaultValue(true);

        builder.Property(x => x.Notes)
            .HasMaxLength(1000);

        builder.HasIndex(x => x.ProductionOrderId);
        builder.HasIndex(x => x.ProductId);
        builder.HasIndex(x => x.WarehouseId);
        builder.HasIndex(x => x.UnitOfMeasureId);
        builder.HasIndex(x => x.StockMovementId);
        builder.HasIndex(x => x.OutputDate);
        builder.HasIndex(x => x.BatchNumber);
        builder.HasIndex(x => x.LotNumber);

        builder.HasOne(x => x.ProductionOrder)
            .WithMany(x => x.Outputs)
            .HasForeignKey(x => x.ProductionOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Warehouse)
            .WithMany()
            .HasForeignKey(x => x.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.UnitOfMeasure)
            .WithMany()
            .HasForeignKey(x => x.UnitOfMeasureId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.StockMovement)
            .WithMany()
            .HasForeignKey(x => x.StockMovementId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
