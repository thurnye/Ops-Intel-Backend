using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class SchedulePlanConfiguration : IEntityTypeConfiguration<SchedulePlan>
{
    public void Configure(EntityTypeBuilder<SchedulePlan> builder)
    {
        builder.ToTable("SchedulePlans", t =>
        {
            t.HasCheckConstraint("CK_SchedulePlan_DateRange", "[PlanningEndDateUtc] >= [PlanningStartDateUtc]");
            t.HasCheckConstraint("CK_SchedulePlan_VersionNumber", "[VersionNumber] >= 1");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.PlanNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .HasMaxLength(1000);

        builder.Property(x => x.TimeZone)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ApprovedBy)
            .HasMaxLength(200);

        builder.Property(x => x.Status)
            .HasConversion<int>();

        builder.Property(x => x.GenerationMode)
            .HasConversion<int>();

        builder.Property(x => x.SchedulingStrategy)
            .HasConversion<int>();

        builder.HasIndex(x => x.PlanNumber)
            .IsUnique();

        builder.HasIndex(x => new { x.WarehouseId, x.Status });

        builder.HasIndex(x => new { x.PlanningStartDateUtc, x.PlanningEndDateUtc });

        builder.HasOne(x => x.Warehouse)
            .WithMany()
            .HasForeignKey(x => x.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ParentPlan)
            .WithMany(x => x.ChildPlans)
            .HasForeignKey(x => x.ParentPlanId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.ScheduleJobs)
            .WithOne(x => x.SchedulePlan)
            .HasForeignKey(x => x.SchedulePlanId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.ScheduleExceptions)
            .WithOne(x => x.SchedulePlan)
            .HasForeignKey(x => x.SchedulePlanId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.ScheduleRevisions)
            .WithOne(x => x.SchedulePlan)
            .HasForeignKey(x => x.SchedulePlanId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.RescheduleHistories)
            .WithOne(x => x.SchedulePlan)
            .HasForeignKey(x => x.SchedulePlanId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.StatusHistories)
            .WithOne(x => x.SchedulePlan)
            .HasForeignKey(x => x.SchedulePlanId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}
