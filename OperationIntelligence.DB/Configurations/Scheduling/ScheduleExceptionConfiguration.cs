using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class ScheduleExceptionConfiguration : IEntityTypeConfiguration<ScheduleException>
{
    public void Configure(EntityTypeBuilder<ScheduleException> builder)
    {
        builder.ToTable("ScheduleExceptions", t =>
        {
            t.HasCheckConstraint("CK_ScheduleException_ResolvedDate", "[ResolvedAtUtc] IS NULL OR [ResolvedAtUtc] >= [DetectedAtUtc]");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ExceptionType)
            .HasConversion<int>();

        builder.Property(x => x.Severity)
            .HasConversion<int>();

        builder.Property(x => x.Status)
            .HasConversion<int>();

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(x => x.AssignedTo)
            .HasMaxLength(200);

        builder.Property(x => x.ResolutionNotes)
            .HasMaxLength(2000);

        builder.HasIndex(x => new { x.Status, x.Severity, x.DetectedAtUtc });

        builder.HasIndex(x => x.SchedulePlanId);
        builder.HasIndex(x => x.ScheduleJobId);
        builder.HasIndex(x => x.ScheduleOperationId);

        builder.HasOne(x => x.SchedulePlan)
            .WithMany(x => x.ScheduleExceptions)
            .HasForeignKey(x => x.SchedulePlanId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ScheduleJob)
            .WithMany(x => x.ScheduleExceptions)
            .HasForeignKey(x => x.ScheduleJobId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ScheduleOperation)
            .WithMany(x => x.ScheduleExceptions)
            .HasForeignKey(x => x.ScheduleOperationId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}
