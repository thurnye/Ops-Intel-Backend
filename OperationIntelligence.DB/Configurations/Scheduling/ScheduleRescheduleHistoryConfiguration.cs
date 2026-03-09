using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class ScheduleRescheduleHistoryConfiguration : IEntityTypeConfiguration<ScheduleRescheduleHistory>
{
    public void Configure(EntityTypeBuilder<ScheduleRescheduleHistory> builder)
    {
        builder.ToTable("ScheduleRescheduleHistories", t =>
        {
            t.HasCheckConstraint("CK_ScheduleRescheduleHistory_OldDateRange", "[OldPlannedEndUtc] IS NULL OR [OldPlannedStartUtc] IS NULL OR [OldPlannedEndUtc] >= [OldPlannedStartUtc]");
            t.HasCheckConstraint("CK_ScheduleRescheduleHistory_NewDateRange", "[NewPlannedEndUtc] IS NULL OR [NewPlannedStartUtc] IS NULL OR [NewPlannedEndUtc] >= [NewPlannedStartUtc]");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ReasonCode)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ReasonDescription)
            .IsRequired()
            .HasMaxLength(1000);

        builder.HasIndex(x => x.ChangedAtUtc);
        builder.HasIndex(x => x.SchedulePlanId);
        builder.HasIndex(x => x.ScheduleJobId);
        builder.HasIndex(x => x.ScheduleOperationId);

        builder.HasOne(x => x.SchedulePlan)
            .WithMany(x => x.RescheduleHistories)
            .HasForeignKey(x => x.SchedulePlanId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ScheduleJob)
            .WithMany(x => x.RescheduleHistories)
            .HasForeignKey(x => x.ScheduleJobId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ScheduleOperation)
            .WithMany(x => x.RescheduleHistories)
            .HasForeignKey(x => x.ScheduleOperationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.OldWorkCenter)
            .WithMany()
            .HasForeignKey(x => x.OldWorkCenterId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.NewWorkCenter)
            .WithMany()
            .HasForeignKey(x => x.NewWorkCenterId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.OldMachine)
            .WithMany()
            .HasForeignKey(x => x.OldMachineId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.NewMachine)
            .WithMany()
            .HasForeignKey(x => x.NewMachineId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}
