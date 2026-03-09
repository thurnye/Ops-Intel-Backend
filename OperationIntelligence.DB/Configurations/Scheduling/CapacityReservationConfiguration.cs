using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class CapacityReservationConfiguration : IEntityTypeConfiguration<CapacityReservation>
{
    public void Configure(EntityTypeBuilder<CapacityReservation> builder)
    {
        builder.ToTable("CapacityReservations", t =>
        {
            t.HasCheckConstraint("CK_CapacityReservation_DateRange", "[ReservedEndUtc] >= [ReservedStartUtc]");
            t.HasCheckConstraint("CK_CapacityReservation_ReservedMinutes", "[ReservedMinutes] >= 0");
            t.HasCheckConstraint("CK_CapacityReservation_AvailableMinutesAtBooking", "[AvailableMinutesAtBooking] >= 0");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ResourceType)
            .HasConversion<int>();

        builder.Property(x => x.Status)
            .HasConversion<int>();

        builder.Property(x => x.ReservationReason)
            .IsRequired()
            .HasMaxLength(300);

        builder.HasIndex(x => new { x.ResourceId, x.ResourceType, x.ReservedStartUtc, x.ReservedEndUtc });

        builder.HasIndex(x => new { x.ScheduleOperationId, x.Status });

        builder.HasOne(x => x.ScheduleOperation)
            .WithMany(x => x.CapacityReservations)
            .HasForeignKey(x => x.ScheduleOperationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Shift)
            .WithMany(x => x.CapacityReservations)
            .HasForeignKey(x => x.ShiftId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}
