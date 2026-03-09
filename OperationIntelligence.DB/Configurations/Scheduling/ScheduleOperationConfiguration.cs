using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class ScheduleOperationConfiguration : IEntityTypeConfiguration<ScheduleOperation>
{
    public void Configure(EntityTypeBuilder<ScheduleOperation> builder)
    {
        builder.ToTable("ScheduleOperations", t =>
        {
            t.HasCheckConstraint("CK_ScheduleOperation_PlannedDateRange", "[PlannedEndUtc] >= [PlannedStartUtc]");
            t.HasCheckConstraint("CK_ScheduleOperation_ActualDateRange", "[ActualEndUtc] IS NULL OR [ActualStartUtc] IS NULL OR [ActualEndUtc] >= [ActualStartUtc]");
            t.HasCheckConstraint("CK_ScheduleOperation_SequenceNo", "[SequenceNo] > 0");
            t.HasCheckConstraint("CK_ScheduleOperation_PriorityScore", "[PriorityScore] >= 0");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.OperationCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.OperationName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.ConstraintReason)
            .HasMaxLength(500);

        builder.Property(x => x.Notes)
            .HasMaxLength(1000);

        builder.Property(x => x.SetupTimeMinutes)
            .HasPrecision(18, 2);

        builder.Property(x => x.RunTimeMinutes)
            .HasPrecision(18, 2);

        builder.Property(x => x.QueueTimeMinutes)
            .HasPrecision(18, 2);

        builder.Property(x => x.WaitTimeMinutes)
            .HasPrecision(18, 2);

        builder.Property(x => x.MoveTimeMinutes)
            .HasPrecision(18, 2);

        builder.Property(x => x.PlannedQuantity)
            .HasPrecision(18, 4);

        builder.Property(x => x.CompletedQuantity)
            .HasPrecision(18, 4);

        builder.Property(x => x.ScrappedQuantity)
            .HasPrecision(18, 4);

        builder.Property(x => x.Status)
            .HasConversion<int>();

        builder.Property(x => x.DispatchStatus)
            .HasConversion<int>();

        builder.HasIndex(x => new { x.ScheduleJobId, x.SequenceNo });

        builder.HasIndex(x => new { x.WorkCenterId, x.PlannedStartUtc, x.PlannedEndUtc });

        builder.HasIndex(x => new { x.MachineId, x.PlannedStartUtc, x.PlannedEndUtc });

        builder.HasIndex(x => new { x.Status, x.DispatchStatus });

        builder.HasOne(x => x.ScheduleJob)
            .WithMany(x => x.ScheduleOperations)
            .HasForeignKey(x => x.ScheduleJobId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.RoutingStep)
            .WithMany()
            .HasForeignKey(x => x.RoutingStepId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.WorkCenter)
            .WithMany()
            .HasForeignKey(x => x.WorkCenterId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Machine)
            .WithMany()
            .HasForeignKey(x => x.MachineId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ProductionExecution)
            .WithMany()
            .HasForeignKey(x => x.ProductionExecutionId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.PlannedShift)
            .WithMany(x => x.PlannedOperations)
            .HasForeignKey(x => x.PlannedShiftId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ActualShift)
            .WithMany(x => x.ActualOperations)
            .HasForeignKey(x => x.ActualShiftId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Constraints)
            .WithOne(x => x.ScheduleOperation)
            .HasForeignKey(x => x.ScheduleOperationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.ResourceOptions)
            .WithOne(x => x.ScheduleOperation)
            .HasForeignKey(x => x.ScheduleOperationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.ResourceAssignments)
            .WithOne(x => x.ScheduleOperation)
            .HasForeignKey(x => x.ScheduleOperationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.CapacityReservations)
            .WithOne(x => x.ScheduleOperation)
            .HasForeignKey(x => x.ScheduleOperationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.DispatchQueueItems)
            .WithOne(x => x.ScheduleOperation)
            .HasForeignKey(x => x.ScheduleOperationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.MaterialChecks)
            .WithOne(x => x.ScheduleOperation)
            .HasForeignKey(x => x.ScheduleOperationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.ScheduleExceptions)
            .WithOne(x => x.ScheduleOperation)
            .HasForeignKey(x => x.ScheduleOperationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.RescheduleHistories)
            .WithOne(x => x.ScheduleOperation)
            .HasForeignKey(x => x.ScheduleOperationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.StatusHistories)
            .WithOne(x => x.ScheduleOperation)
            .HasForeignKey(x => x.ScheduleOperationId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}
