using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class ScheduleResourceAssignmentConfiguration : IEntityTypeConfiguration<ScheduleResourceAssignment>
{
    public void Configure(EntityTypeBuilder<ScheduleResourceAssignment> builder)
    {
        builder.ToTable("ScheduleResourceAssignments", t =>
        {
            t.HasCheckConstraint("CK_ScheduleResourceAssignment_DateRange", "[AssignedEndUtc] >= [AssignedStartUtc]");
            t.HasCheckConstraint("CK_ScheduleResourceAssignment_PlannedHours", "[PlannedHours] >= 0");
            t.HasCheckConstraint("CK_ScheduleResourceAssignment_ActualHours", "[ActualHours] >= 0");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ResourceType)
            .HasConversion<int>();

        builder.Property(x => x.AssignmentRole)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.PlannedHours)
            .HasPrecision(18, 2);

        builder.Property(x => x.ActualHours)
            .HasPrecision(18, 2);

        builder.Property(x => x.Status)
            .HasConversion<int>();

        builder.Property(x => x.Notes)
            .HasMaxLength(1000);

        builder.HasIndex(x => new { x.ScheduleOperationId, x.ResourceId, x.ResourceType });

        builder.HasIndex(x => new { x.AssignedStartUtc, x.AssignedEndUtc });

        builder.HasOne(x => x.ScheduleOperation)
            .WithMany(x => x.ResourceAssignments)
            .HasForeignKey(x => x.ScheduleOperationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Shift)
            .WithMany(x => x.ResourceAssignments)
            .HasForeignKey(x => x.ShiftId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}
