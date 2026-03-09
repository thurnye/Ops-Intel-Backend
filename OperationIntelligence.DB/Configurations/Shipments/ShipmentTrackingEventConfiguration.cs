using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class ShipmentTrackingEventConfiguration : IEntityTypeConfiguration<ShipmentTrackingEvent>
{
    public void Configure(EntityTypeBuilder<ShipmentTrackingEvent> builder)
    {
        builder.ToTable("ShipmentTrackingEvents");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.EventCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.EventName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.Description).HasMaxLength(1000);
        builder.Property(x => x.LocationName).HasMaxLength(150);
        builder.Property(x => x.City).HasMaxLength(100);
        builder.Property(x => x.StateOrProvince).HasMaxLength(100);
        builder.Property(x => x.Country).HasMaxLength(100);
        builder.Property(x => x.CarrierStatusCode).HasMaxLength(50);

        builder.Property(x => x.Source)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(x => x.ShipmentId);
        builder.HasIndex(x => new { x.ShipmentId, x.EventTimeUtc });
        builder.HasIndex(x => new { x.EventCode, x.EventTimeUtc });

        builder.HasOne(x => x.Shipment)
            .WithMany(x => x.TrackingEvents)
            .HasForeignKey(x => x.ShipmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
