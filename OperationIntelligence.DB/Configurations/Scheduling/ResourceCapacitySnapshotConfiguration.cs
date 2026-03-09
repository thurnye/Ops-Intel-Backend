using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class ResourceCapacitySnapshotConfiguration : IEntityTypeConfiguration<ResourceCapacitySnapshot>
{
    public void Configure(EntityTypeBuilder<ResourceCapacitySnapshot> builder)
    {
        builder.ToTable("ResourceCapacitySnapshots", t =>
        {
            t.HasCheckConstraint("CK_ResourceCapacitySnapshot_TotalCapacityMinutes", "[TotalCapacityMinutes] >= 0");
            t.HasCheckConstraint("CK_ResourceCapacitySnapshot_ReservedMinutes", "[ReservedMinutes] >= 0");
            t.HasCheckConstraint("CK_ResourceCapacitySnapshot_AvailableMinutes", "[AvailableMinutes] >= 0");
            t.HasCheckConstraint("CK_ResourceCapacitySnapshot_OvertimeMinutes", "[OvertimeMinutes] >= 0");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ResourceType)
            .HasConversion<int>();

        builder.HasIndex(x => new { x.ResourceId, x.ResourceType, x.SnapshotDateUtc });

        builder.HasIndex(x => new { x.ShiftId, x.SnapshotDateUtc });

        builder.HasOne(x => x.Shift)
            .WithMany(x => x.CapacitySnapshots)
            .HasForeignKey(x => x.ShiftId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}
