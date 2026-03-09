using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class ScheduleRevisionConfiguration : IEntityTypeConfiguration<ScheduleRevision>
{
    public void Configure(EntityTypeBuilder<ScheduleRevision> builder)
    {
        builder.ToTable("ScheduleRevisions", t =>
        {
            t.HasCheckConstraint("CK_ScheduleRevision_RevisionNo", "[RevisionNo] > 0");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.RevisionType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ChangeSummary)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(x => x.Reason)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(x => x.SnapshotJson)
            .HasColumnType("nvarchar(max)");

        builder.HasIndex(x => new { x.SchedulePlanId, x.RevisionNo })
            .IsUnique();

        builder.HasOne(x => x.SchedulePlan)
            .WithMany(x => x.ScheduleRevisions)
            .HasForeignKey(x => x.SchedulePlanId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}
