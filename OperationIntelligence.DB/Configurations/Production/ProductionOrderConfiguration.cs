using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class ProductionOrderConfiguration : IEntityTypeConfiguration<ProductionOrder>
{
    public void Configure(EntityTypeBuilder<ProductionOrder> builder)
    {
        builder.ToTable("ProductionOrders");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ProductionOrderNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.PlannedQuantity)
            .HasPrecision(18, 4);

        builder.Property(x => x.ProducedQuantity)
            .HasPrecision(18, 4);

        builder.Property(x => x.ScrapQuantity)
            .HasPrecision(18, 4);

        builder.Property(x => x.RemainingQuantity)
            .HasPrecision(18, 4);

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.Priority)
            .IsRequired();

        builder.Property(x => x.SourceType)
            .IsRequired();

        builder.Property(x => x.BatchNumber)
            .HasMaxLength(100);

        builder.Property(x => x.LotNumber)
            .HasMaxLength(100);

        builder.Property(x => x.Notes)
            .HasMaxLength(2000);

        builder.Property(x => x.EstimatedMaterialCost)
            .HasPrecision(18, 2);

        builder.Property(x => x.EstimatedLaborCost)
            .HasPrecision(18, 2);

        builder.Property(x => x.EstimatedOverheadCost)
            .HasPrecision(18, 2);

        builder.Property(x => x.ActualMaterialCost)
            .HasPrecision(18, 2);

        builder.Property(x => x.ActualLaborCost)
            .HasPrecision(18, 2);

        builder.Property(x => x.ActualOverheadCost)
            .HasPrecision(18, 2);

        builder.Property(x => x.IsReleased)
            .HasDefaultValue(false);

        builder.Property(x => x.IsClosed)
            .HasDefaultValue(false);

        builder.HasIndex(x => x.ProductionOrderNumber)
            .IsUnique();

        builder.HasIndex(x => x.ProductId);
        builder.HasIndex(x => x.UnitOfMeasureId);
        builder.HasIndex(x => x.BillOfMaterialId);
        builder.HasIndex(x => x.RoutingId);
        builder.HasIndex(x => x.WarehouseId);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.PlannedStartDate);
        builder.HasIndex(x => new { x.SourceType, x.SourceReferenceId });

        builder.HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.UnitOfMeasure)
            .WithMany()
            .HasForeignKey(x => x.UnitOfMeasureId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.BillOfMaterial)
            .WithMany(x => x.ProductionOrders)
            .HasForeignKey(x => x.BillOfMaterialId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Routing)
            .WithMany(x => x.ProductionOrders)
            .HasForeignKey(x => x.RoutingId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Warehouse)
            .WithMany()
            .HasForeignKey(x => x.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
