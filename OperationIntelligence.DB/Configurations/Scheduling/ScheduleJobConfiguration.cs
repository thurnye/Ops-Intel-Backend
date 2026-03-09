using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class ScheduleJobConfiguration : IEntityTypeConfiguration<ScheduleJob>
{
    [Obsolete]
    public void Configure(EntityTypeBuilder<ScheduleJob> builder)
    {
        builder.ToTable("ScheduleJobs", t =>
        {
            t.HasCheckConstraint("CK_ScheduleJob_PlannedQuantity", "[PlannedQuantity] >= 0");
            t.HasCheckConstraint("CK_ScheduleJob_CompletedQuantity", "[CompletedQuantity] >= 0");
            t.HasCheckConstraint("CK_ScheduleJob_ScrappedQuantity", "[ScrappedQuantity] >= 0");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.JobNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.JobName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Notes)
            .HasMaxLength(1000);

        builder.Property(x => x.PlannedQuantity)
            .HasPrecision(18, 4);

        builder.Property(x => x.CompletedQuantity)
            .HasPrecision(18, 4);

        builder.Property(x => x.ScrappedQuantity)
            .HasPrecision(18, 4);

        builder.Property(x => x.Priority)
            .HasConversion<int>();

        builder.Property(x => x.Status)
            .HasConversion<int>();

        builder.Property(x => x.MaterialReadinessStatus)
            .HasConversion<int>();

        builder.HasIndex(x => x.JobNumber)
            .IsUnique();

        builder.HasIndex(x => new { x.SchedulePlanId, x.Status, x.Priority });

        builder.HasIndex(x => new { x.ProductionOrderId, x.ProductId });

        builder.HasIndex(x => x.DueDateUtc);

        builder.HasOne(x => x.SchedulePlan)
            .WithMany(x => x.ScheduleJobs)
            .HasForeignKey(x => x.SchedulePlanId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ProductionOrder)
            .WithMany()
            .HasForeignKey(x => x.ProductionOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Order)
            .WithMany()
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.OrderItem)
            .WithMany()
            .HasForeignKey(x => x.OrderItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Warehouse)
            .WithMany()
            .HasForeignKey(x => x.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.ScheduleOperations)
            .WithOne(x => x.ScheduleJob)
            .HasForeignKey(x => x.ScheduleJobId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.MaterialChecks)
            .WithOne(x => x.ScheduleJob)
            .HasForeignKey(x => x.ScheduleJobId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.ScheduleExceptions)
            .WithOne(x => x.ScheduleJob)
            .HasForeignKey(x => x.ScheduleJobId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.RescheduleHistories)
            .WithOne(x => x.ScheduleJob)
            .HasForeignKey(x => x.ScheduleJobId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.StatusHistories)
            .WithOne(x => x.ScheduleJob)
            .HasForeignKey(x => x.ScheduleJobId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}
