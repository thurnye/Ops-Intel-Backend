using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class ScheduleOperationDependencyConfiguration : IEntityTypeConfiguration<ScheduleOperationDependency>
{
    public void Configure(EntityTypeBuilder<ScheduleOperationDependency> builder)
    {
        builder.ToTable("ScheduleOperationDependencies", t =>
        {
            t.HasCheckConstraint("CK_ScheduleOperationDependency_LagMinutes", "[LagMinutes] >= 0");
            t.HasCheckConstraint("CK_ScheduleOperationDependency_NoSelfReference", "[PredecessorOperationId] <> [SuccessorOperationId]");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.DependencyType)
            .HasConversion<int>();

        builder.HasIndex(x => new { x.PredecessorOperationId, x.SuccessorOperationId })
            .IsUnique();

        builder.HasOne(x => x.PredecessorOperation)
            .WithMany(x => x.PredecessorDependencies)
            .HasForeignKey(x => x.PredecessorOperationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.SuccessorOperation)
            .WithMany(x => x.SuccessorDependencies)
            .HasForeignKey(x => x.SuccessorOperationId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}
