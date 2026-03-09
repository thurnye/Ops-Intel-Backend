using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class ProductionExecutionConfiguration : IEntityTypeConfiguration<ProductionExecution>
{
    public void Configure(EntityTypeBuilder<ProductionExecution> builder)
    {
        builder.ToTable("ProductionExecutions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.PlannedQuantity)
            .HasPrecision(18, 4);

        builder.Property(x => x.CompletedQuantity)
            .HasPrecision(18, 4);

        builder.Property(x => x.ScrapQuantity)
            .HasPrecision(18, 4);

        builder.Property(x => x.ActualSetupTimeMinutes)
            .HasPrecision(18, 2);

        builder.Property(x => x.ActualRunTimeMinutes)
            .HasPrecision(18, 2);

        builder.Property(x => x.ActualDowntimeMinutes)
            .HasPrecision(18, 2);

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.Remarks)
            .HasMaxLength(2000);

        builder.HasIndex(x => x.ProductionOrderId);
        builder.HasIndex(x => x.RoutingStepId);
        builder.HasIndex(x => x.WorkCenterId);
        builder.HasIndex(x => x.MachineId);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.PlannedStartDate);

        builder.HasOne(x => x.ProductionOrder)
            .WithMany(x => x.Executions)
            .HasForeignKey(x => x.ProductionOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.RoutingStep)
            .WithMany(x => x.ProductionExecutions)
            .HasForeignKey(x => x.RoutingStepId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.WorkCenter)
            .WithMany(x => x.ProductionExecutions)
            .HasForeignKey(x => x.WorkCenterId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Machine)
            .WithMany(x => x.ProductionExecutions)
            .HasForeignKey(x => x.MachineId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
