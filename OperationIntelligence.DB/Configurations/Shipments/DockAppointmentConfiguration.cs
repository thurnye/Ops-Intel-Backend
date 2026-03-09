using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class DockAppointmentConfiguration : IEntityTypeConfiguration<DockAppointment>
{
    public void Configure(EntityTypeBuilder<DockAppointment> builder)
    {
        builder.ToTable("DockAppointments", t =>
        {
            t.HasCheckConstraint(
                "CK_DockAppointments_ScheduledWindow",
                "[ScheduledEndUtc] >= [ScheduledStartUtc]");
            t.HasCheckConstraint(
                "CK_DockAppointments_ActualWindow",
                "[ActualDepartureUtc] IS NULL OR [ActualArrivalUtc] IS NULL OR [ActualDepartureUtc] >= [ActualArrivalUtc]");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.AppointmentNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.DockCode).HasMaxLength(50);
        builder.Property(x => x.TrailerNumber).HasMaxLength(100);
        builder.Property(x => x.DriverName).HasMaxLength(150);
        builder.Property(x => x.Notes).HasMaxLength(1000);

        builder.Property(x => x.Status).HasConversion<int>();

        builder.HasIndex(x => x.AppointmentNumber).IsUnique();
        builder.HasIndex(x => new { x.WarehouseId, x.ScheduledStartUtc });
        builder.HasIndex(x => new { x.WarehouseId, x.DockCode, x.ScheduledStartUtc });

        builder.HasOne(x => x.Warehouse)
            .WithMany(x => x.DockAppointments)
            .HasForeignKey(x => x.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Carrier)
            .WithMany(x => x.DockAppointments)
            .HasForeignKey(x => x.CarrierId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Shipments)
            .WithOne(x => x.DockAppointment)
            .HasForeignKey(x => x.DockAppointmentId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
