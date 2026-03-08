using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class StockMovementConfiguration : IEntityTypeConfiguration<StockMovement>
{
    public void Configure(EntityTypeBuilder<StockMovement> builder)
    {
        builder.ToTable("StockMovements");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Quantity).HasColumnType("decimal(18,2)");
        builder.Property(x => x.QuantityBefore).HasColumnType("decimal(18,2)");
        builder.Property(x => x.QuantityAfter).HasColumnType("decimal(18,2)");

        builder.Property(x => x.ReferenceNumber).HasMaxLength(100);
        builder.Property(x => x.Reason).HasMaxLength(500);
        builder.Property(x => x.Notes).HasMaxLength(2000);

        builder.HasIndex(x => x.ReferenceNumber);
        builder.HasIndex(x => x.MovementDateUtc);
        builder.HasIndex(x => new { x.ProductId, x.WarehouseId, x.MovementDateUtc });

        builder.HasOne(x => x.Product)
            .WithMany(x => x.StockMovements)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Warehouse)
            .WithMany(x => x.StockMovements)
            .HasForeignKey(x => x.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.RelatedWarehouse)
            .WithMany()
            .HasForeignKey(x => x.RelatedWarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}