using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class DeliveryRunConfiguration : IEntityTypeConfiguration<DeliveryRun>
{
    public void Configure(EntityTypeBuilder<DeliveryRun> builder)
    {
        builder.ToTable("DeliveryRuns", t =>
        {
            t.HasCheckConstraint(
                "CK_DeliveryRuns_PlannedWindow",
                "[PlannedEndUtc] >= [PlannedStartUtc]");
            t.HasCheckConstraint(
                "CK_DeliveryRuns_ActualWindow",
                "[ActualEndUtc] IS NULL OR [ActualStartUtc] IS NULL OR [ActualEndUtc] >= [ActualStartUtc]");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.RunNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.DriverName).HasMaxLength(150);
        builder.Property(x => x.VehicleNumber).HasMaxLength(100);
        builder.Property(x => x.RouteCode).HasMaxLength(100);
        builder.Property(x => x.Notes).HasMaxLength(1000);

        builder.Property(x => x.Status).HasConversion<int>();

        builder.HasIndex(x => x.RunNumber).IsUnique();
        builder.HasIndex(x => new { x.WarehouseId, x.PlannedStartUtc });
        builder.HasIndex(x => new { x.Status, x.PlannedStartUtc });

        builder.HasOne(x => x.Warehouse)
            .WithMany(x => x.DeliveryRuns)
            .HasForeignKey(x => x.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Shipments)
            .WithOne(x => x.DeliveryRun)
            .HasForeignKey(x => x.DeliveryRunId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
