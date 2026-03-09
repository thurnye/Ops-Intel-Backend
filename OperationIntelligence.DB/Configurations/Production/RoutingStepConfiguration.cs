using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class RoutingStepConfiguration : IEntityTypeConfiguration<RoutingStep>
{
    public void Configure(EntityTypeBuilder<RoutingStep> builder)
    {
        builder.ToTable("RoutingSteps");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.OperationCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.OperationName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.SetupTimeMinutes)
            .HasPrecision(18, 2);

        builder.Property(x => x.RunTimeMinutesPerUnit)
            .HasPrecision(18, 4);

        builder.Property(x => x.QueueTimeMinutes)
            .HasPrecision(18, 2);

        builder.Property(x => x.WaitTimeMinutes)
            .HasPrecision(18, 2);

        builder.Property(x => x.MoveTimeMinutes)
            .HasPrecision(18, 2);

        builder.Property(x => x.Instructions)
            .HasMaxLength(2000);

        builder.Property(x => x.Notes)
            .HasMaxLength(1000);

        builder.HasIndex(x => x.RoutingId);

        builder.HasIndex(x => x.WorkCenterId);

        builder.HasIndex(x => new { x.RoutingId, x.Sequence })
            .IsUnique();

        builder.HasOne(x => x.Routing)
            .WithMany(x => x.Steps)
            .HasForeignKey(x => x.RoutingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.WorkCenter)
            .WithMany(x => x.RoutingSteps)
            .HasForeignKey(x => x.WorkCenterId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
