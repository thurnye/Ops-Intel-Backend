using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class DispatchQueueItemConfiguration : IEntityTypeConfiguration<DispatchQueueItem>
{
    public void Configure(EntityTypeBuilder<DispatchQueueItem> builder)
    {
        builder.ToTable("DispatchQueueItems", t =>
        {
            t.HasCheckConstraint("CK_DispatchQueueItem_QueuePosition", "[QueuePosition] > 0");
            t.HasCheckConstraint("CK_DispatchQueueItem_PriorityScore", "[PriorityScore] >= 0");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Status)
            .HasConversion<int>();

        builder.Property(x => x.DispatchNotes)
            .HasMaxLength(1000);

        builder.HasIndex(x => new { x.WorkCenterId, x.QueuePosition, x.IsActive });

        builder.HasIndex(x => new { x.MachineId, x.QueuePosition, x.IsActive });

        builder.HasIndex(x => new { x.ScheduleOperationId, x.IsActive });

        builder.HasOne(x => x.ScheduleOperation)
            .WithMany(x => x.DispatchQueueItems)
            .HasForeignKey(x => x.ScheduleOperationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.WorkCenter)
            .WithMany()
            .HasForeignKey(x => x.WorkCenterId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Machine)
            .WithMany()
            .HasForeignKey(x => x.MachineId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}
