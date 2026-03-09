using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class ScheduleOperationConstraintConfiguration : IEntityTypeConfiguration<ScheduleOperationConstraint>
{
    public void Configure(EntityTypeBuilder<ScheduleOperationConstraint> builder)
    {
        builder.ToTable("ScheduleOperationConstraints");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ConstraintType)
            .HasConversion<int>();

        builder.Property(x => x.ReferenceNo)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.HasIndex(x => new { x.ScheduleOperationId, x.ConstraintType });

        builder.HasOne(x => x.ScheduleOperation)
            .WithMany(x => x.Constraints)
            .HasForeignKey(x => x.ScheduleOperationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
