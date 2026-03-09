using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class ShipmentConfiguration : IEntityTypeConfiguration<Shipment>
{
    public void Configure(EntityTypeBuilder<Shipment> builder)
    {
        builder.ToTable("Shipments", t =>
        {
            t.HasCheckConstraint("CK_Shipments_TotalPackages", "[TotalPackages] >= 0");
            t.HasCheckConstraint("CK_Shipments_TotalWeight", "[TotalWeight] >= 0");
            t.HasCheckConstraint("CK_Shipments_TotalVolume", "[TotalVolume] >= 0");
            t.HasCheckConstraint("CK_Shipments_FreightCost", "[FreightCost] >= 0");
            t.HasCheckConstraint("CK_Shipments_InsuranceCost", "[InsuranceCost] >= 0");
            t.HasCheckConstraint("CK_Shipments_OtherCharges", "[OtherCharges] >= 0");
            t.HasCheckConstraint("CK_Shipments_TotalShippingCost", "[TotalShippingCost] >= 0");
            t.HasCheckConstraint(
                "CK_Shipments_PickupWindow",
                "[ScheduledPickupEndUtc] IS NULL OR [ScheduledPickupStartUtc] IS NULL OR [ScheduledPickupEndUtc] >= [ScheduledPickupStartUtc]");
            t.HasCheckConstraint(
                "CK_Shipments_PlannedDates",
                "[PlannedDeliveryDateUtc] IS NULL OR [PlannedShipDateUtc] IS NULL OR [PlannedDeliveryDateUtc] >= [PlannedShipDateUtc]");
            t.HasCheckConstraint(
                "CK_Shipments_ActualDates",
                "[ActualDeliveryDateUtc] IS NULL OR [ActualShipDateUtc] IS NULL OR [ActualDeliveryDateUtc] >= [ActualShipDateUtc]");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ShipmentNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.CustomerReference).HasMaxLength(100);
        builder.Property(x => x.ExternalReference).HasMaxLength(100);
        builder.Property(x => x.TrackingNumber).HasMaxLength(100);
        builder.Property(x => x.MasterTrackingNumber).HasMaxLength(100);
        builder.Property(x => x.CurrencyCode).IsRequired().HasMaxLength(10);
        builder.Property(x => x.ShippingTerms).HasMaxLength(100);
        builder.Property(x => x.Incoterm).HasMaxLength(50);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.Property(x => x.InternalNotes).HasMaxLength(1000);

        builder.Property(x => x.Type).HasConversion<int>();
        builder.Property(x => x.Status).HasConversion<int>();
        builder.Property(x => x.Priority).HasConversion<int>();

        builder.Property(x => x.TotalWeight).HasPrecision(18, 4);
        builder.Property(x => x.TotalVolume).HasPrecision(18, 4);
        builder.Property(x => x.FreightCost).HasPrecision(18, 2);
        builder.Property(x => x.InsuranceCost).HasPrecision(18, 2);
        builder.Property(x => x.OtherCharges).HasPrecision(18, 2);
        builder.Property(x => x.TotalShippingCost).HasPrecision(18, 2);

        builder.HasIndex(x => x.ShipmentNumber).IsUnique();
        builder.HasIndex(x => x.TrackingNumber);
        builder.HasIndex(x => x.MasterTrackingNumber);
        builder.HasIndex(x => new { x.Status, x.PlannedShipDateUtc });
        builder.HasIndex(x => new { x.OrderId, x.Status });
        builder.HasIndex(x => new { x.WarehouseId, x.Status });

        builder.HasOne(x => x.Order)
            .WithMany(x => x.Shipments)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Warehouse)
            .WithMany(x => x.Shipments)
            .HasForeignKey(x => x.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Carrier)
            .WithMany(x => x.Shipments)
            .HasForeignKey(x => x.CarrierId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CarrierService)
            .WithMany(x => x.Shipments)
            .HasForeignKey(x => x.CarrierServiceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.OriginAddress)
            .WithMany(x => x.OriginShipments)
            .HasForeignKey(x => x.OriginAddressId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.DestinationAddress)
            .WithMany(x => x.DestinationShipments)
            .HasForeignKey(x => x.DestinationAddressId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.DeliveryRun)
            .WithMany(x => x.Shipments)
            .HasForeignKey(x => x.DeliveryRunId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.DockAppointment)
            .WithMany(x => x.Shipments)
            .HasForeignKey(x => x.DockAppointmentId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(x => x.Items)
            .WithOne(x => x.Shipment)
            .HasForeignKey(x => x.ShipmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Packages)
            .WithOne(x => x.Shipment)
            .HasForeignKey(x => x.ShipmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.TrackingEvents)
            .WithOne(x => x.Shipment)
            .HasForeignKey(x => x.ShipmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Documents)
            .WithOne(x => x.Shipment)
            .HasForeignKey(x => x.ShipmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.StatusHistories)
            .WithOne(x => x.Shipment)
            .HasForeignKey(x => x.ShipmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Exceptions)
            .WithOne(x => x.Shipment)
            .HasForeignKey(x => x.ShipmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Charges)
            .WithOne(x => x.Shipment)
            .HasForeignKey(x => x.ShipmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Insurances)
            .WithOne(x => x.Shipment)
            .HasForeignKey(x => x.ShipmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.CustomsDocuments)
            .WithOne(x => x.Shipment)
            .HasForeignKey(x => x.ShipmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.ReturnShipments)
            .WithOne(x => x.Shipment)
            .HasForeignKey(x => x.ShipmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
