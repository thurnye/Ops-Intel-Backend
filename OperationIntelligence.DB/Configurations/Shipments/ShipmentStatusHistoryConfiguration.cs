using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class ShipmentStatusHistoryConfiguration : IEntityTypeConfiguration<ShipmentStatusHistory>
{
    public void Configure(EntityTypeBuilder<ShipmentStatusHistory> builder)
    {
        builder.ToTable("ShipmentStatusHistories");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.FromStatus).HasConversion<int>();
        builder.Property(x => x.ToStatus).HasConversion<int>();

        builder.Property(x => x.ChangedBy)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.Reason).HasMaxLength(500);

        builder.HasIndex(x => x.ShipmentId);
        builder.HasIndex(x => new { x.ShipmentId, x.ChangedAtUtc });

        builder.HasOne(x => x.Shipment)
            .WithMany(x => x.StatusHistories)
            .HasForeignKey(x => x.ShipmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
