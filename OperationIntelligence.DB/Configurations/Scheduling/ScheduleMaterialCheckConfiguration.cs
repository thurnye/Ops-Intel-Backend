using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class ScheduleMaterialCheckConfiguration : IEntityTypeConfiguration<ScheduleMaterialCheck>
{
    public void Configure(EntityTypeBuilder<ScheduleMaterialCheck> builder)
    {
        builder.ToTable("ScheduleMaterialChecks", t =>
        {
            t.HasCheckConstraint("CK_ScheduleMaterialCheck_RequiredQuantity", "[RequiredQuantity] >= 0");
            t.HasCheckConstraint("CK_ScheduleMaterialCheck_AvailableQuantity", "[AvailableQuantity] >= 0");
            t.HasCheckConstraint("CK_ScheduleMaterialCheck_ReservedQuantity", "[ReservedQuantity] >= 0");
            t.HasCheckConstraint("CK_ScheduleMaterialCheck_ShortageQuantity", "[ShortageQuantity] >= 0");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.RequiredQuantity)
            .HasPrecision(18, 4);

        builder.Property(x => x.AvailableQuantity)
            .HasPrecision(18, 4);

        builder.Property(x => x.ReservedQuantity)
            .HasPrecision(18, 4);

        builder.Property(x => x.ShortageQuantity)
            .HasPrecision(18, 4);

        builder.Property(x => x.Status)
            .HasConversion<int>();

        builder.Property(x => x.Notes)
            .HasMaxLength(1000);

        builder.HasIndex(x => new { x.ScheduleJobId, x.MaterialProductId });

        builder.HasIndex(x => new { x.ProductionOrderId, x.RoutingStepId });

        builder.HasOne(x => x.ScheduleJob)
            .WithMany(x => x.MaterialChecks)
            .HasForeignKey(x => x.ScheduleJobId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ScheduleOperation)
            .WithMany(x => x.MaterialChecks)
            .HasForeignKey(x => x.ScheduleOperationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ProductionOrder)
            .WithMany()
            .HasForeignKey(x => x.ProductionOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.RoutingStep)
            .WithMany()
            .HasForeignKey(x => x.RoutingStepId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.MaterialProduct)
            .WithMany()
            .HasForeignKey(x => x.MaterialProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Warehouse)
            .WithMany()
            .HasForeignKey(x => x.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}
