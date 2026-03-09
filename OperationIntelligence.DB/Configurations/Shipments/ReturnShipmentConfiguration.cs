using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class ReturnShipmentConfiguration : IEntityTypeConfiguration<ReturnShipment>
{
    public void Configure(EntityTypeBuilder<ReturnShipment> builder)
    {
        builder.ToTable("ReturnShipments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ReturnShipmentNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.TrackingNumber).HasMaxLength(100);

        builder.Property(x => x.ReasonCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.ReasonDescription).HasMaxLength(1000);
        builder.Property(x => x.Notes).HasMaxLength(1000);

        builder.Property(x => x.Status).HasConversion<int>();

        builder.HasIndex(x => x.ReturnShipmentNumber).IsUnique();
        builder.HasIndex(x => x.TrackingNumber);
        builder.HasIndex(x => x.ShipmentId);
        builder.HasIndex(x => new { x.OrderId, x.Status });

        builder.HasOne(x => x.Shipment)
            .WithMany(x => x.ReturnShipments)
            .HasForeignKey(x => x.ShipmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Order)
            .WithMany(x => x.ReturnShipments)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.OriginAddress)
            .WithMany(x => x.OriginReturnShipments)
            .HasForeignKey(x => x.OriginAddressId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.DestinationAddress)
            .WithMany(x => x.DestinationReturnShipments)
            .HasForeignKey(x => x.DestinationAddressId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Carrier)
            .WithMany(x => x.ReturnShipments)
            .HasForeignKey(x => x.CarrierId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CarrierService)
            .WithMany(x => x.ReturnShipments)
            .HasForeignKey(x => x.CarrierServiceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Items)
            .WithOne(x => x.ReturnShipment)
            .HasForeignKey(x => x.ReturnShipmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
