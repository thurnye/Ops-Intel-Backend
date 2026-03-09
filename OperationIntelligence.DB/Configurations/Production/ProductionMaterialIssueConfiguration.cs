using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class ProductionMaterialIssueConfiguration : IEntityTypeConfiguration<ProductionMaterialIssue>
{
    public void Configure(EntityTypeBuilder<ProductionMaterialIssue> builder)
    {
        builder.ToTable("ProductionMaterialIssues");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.PlannedQuantity)
            .HasPrecision(18, 4);

        builder.Property(x => x.IssuedQuantity)
            .HasPrecision(18, 4);

        builder.Property(x => x.ReturnedQuantity)
            .HasPrecision(18, 4);

        builder.Property(x => x.BatchNumber)
            .HasMaxLength(100);

        builder.Property(x => x.LotNumber)
            .HasMaxLength(100);

        builder.Property(x => x.Notes)
            .HasMaxLength(1000);

        builder.HasIndex(x => x.ProductionOrderId);
        builder.HasIndex(x => x.MaterialProductId);
        builder.HasIndex(x => x.WarehouseId);
        builder.HasIndex(x => x.UnitOfMeasureId);
        builder.HasIndex(x => x.StockMovementId);
        builder.HasIndex(x => x.IssueDate);

        builder.HasOne(x => x.ProductionOrder)
            .WithMany(x => x.MaterialIssues)
            .HasForeignKey(x => x.ProductionOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.MaterialProduct)
            .WithMany()
            .HasForeignKey(x => x.MaterialProductId)
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
