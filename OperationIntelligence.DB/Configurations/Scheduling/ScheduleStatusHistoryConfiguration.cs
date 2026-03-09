using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class ScheduleStatusHistoryConfiguration : IEntityTypeConfiguration<ScheduleStatusHistory>
{
    public void Configure(EntityTypeBuilder<ScheduleStatusHistory> builder)
    {
        builder.ToTable("ScheduleStatusHistories");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.EntityType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.OldStatus)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.NewStatus)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Reason)
            .HasMaxLength(500);

        builder.Property(x => x.Notes)
            .HasMaxLength(1000);

        builder.HasIndex(x => x.ChangedAtUtc);
        builder.HasIndex(x => x.SchedulePlanId);
        builder.HasIndex(x => x.ScheduleJobId);
        builder.HasIndex(x => x.ScheduleOperationId);

        builder.HasOne(x => x.SchedulePlan)
            .WithMany(x => x.StatusHistories)
            .HasForeignKey(x => x.SchedulePlanId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ScheduleJob)
            .WithMany(x => x.StatusHistories)
            .HasForeignKey(x => x.ScheduleJobId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ScheduleOperation)
            .WithMany(x => x.StatusHistories)
            .HasForeignKey(x => x.ScheduleOperationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
