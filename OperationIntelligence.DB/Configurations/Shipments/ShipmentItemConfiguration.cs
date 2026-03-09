using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class ShipmentItemConfiguration : IEntityTypeConfiguration<ShipmentItem>
{
    public void Configure(EntityTypeBuilder<ShipmentItem> builder)
    {
        builder.ToTable("ShipmentItems", t =>
        {
            t.HasCheckConstraint("CK_ShipmentItems_OrderedQuantity", "[OrderedQuantity] >= 0");
            t.HasCheckConstraint("CK_ShipmentItems_AllocatedQuantity", "[AllocatedQuantity] >= 0");
            t.HasCheckConstraint("CK_ShipmentItems_PickedQuantity", "[PickedQuantity] >= 0");
            t.HasCheckConstraint("CK_ShipmentItems_PackedQuantity", "[PackedQuantity] >= 0");
            t.HasCheckConstraint("CK_ShipmentItems_ShippedQuantity", "[ShippedQuantity] >= 0");
            t.HasCheckConstraint("CK_ShipmentItems_DeliveredQuantity", "[DeliveredQuantity] >= 0");
            t.HasCheckConstraint("CK_ShipmentItems_ReturnedQuantity", "[ReturnedQuantity] >= 0");
            t.HasCheckConstraint("CK_ShipmentItems_UnitWeight", "[UnitWeight] >= 0");
            t.HasCheckConstraint("CK_ShipmentItems_UnitVolume", "[UnitVolume] >= 0");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.LineNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.LotNumber).HasMaxLength(100);
        builder.Property(x => x.SerialNumber).HasMaxLength(100);
        builder.Property(x => x.Notes).HasMaxLength(1000);

        builder.Property(x => x.Status).HasConversion<int>();

        builder.Property(x => x.OrderedQuantity).HasPrecision(18, 4);
        builder.Property(x => x.AllocatedQuantity).HasPrecision(18, 4);
        builder.Property(x => x.PickedQuantity).HasPrecision(18, 4);
        builder.Property(x => x.PackedQuantity).HasPrecision(18, 4);
        builder.Property(x => x.ShippedQuantity).HasPrecision(18, 4);
        builder.Property(x => x.DeliveredQuantity).HasPrecision(18, 4);
        builder.Property(x => x.ReturnedQuantity).HasPrecision(18, 4);
        builder.Property(x => x.UnitWeight).HasPrecision(18, 4);
        builder.Property(x => x.UnitVolume).HasPrecision(18, 4);

        builder.HasIndex(x => new { x.ShipmentId, x.LineNumber }).IsUnique();
        builder.HasIndex(x => x.OrderItemId);
        builder.HasIndex(x => x.ProductId);
        builder.HasIndex(x => x.InventoryStockId);
        builder.HasIndex(x => x.ProductionOrderId);
        builder.HasIndex(x => new { x.ShipmentId, x.Status });

        builder.HasOne(x => x.Shipment)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.ShipmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.OrderItem)
            .WithMany(x => x.ShipmentItems)
            .HasForeignKey(x => x.OrderItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Product)
            .WithMany(x => x.ShipmentItems)
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

        builder.HasOne(x => x.InventoryStock)
            .WithMany()
            .HasForeignKey(x => x.InventoryStockId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ProductionOrder)
            .WithMany()
            .HasForeignKey(x => x.ProductionOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.PackageItems)
            .WithOne(x => x.ShipmentItem)
            .HasForeignKey(x => x.ShipmentItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.ReturnItems)
            .WithOne(x => x.ShipmentItem)
            .HasForeignKey(x => x.ShipmentItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
