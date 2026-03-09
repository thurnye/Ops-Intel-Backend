using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class ProductionMaterialConsumptionConfiguration : IEntityTypeConfiguration<ProductionMaterialConsumption>
{
    public void Configure(EntityTypeBuilder<ProductionMaterialConsumption> builder)
    {
        builder.ToTable("ProductionMaterialConsumptions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ConsumedQuantity)
            .HasPrecision(18, 4);

        builder.Property(x => x.Notes)
            .HasMaxLength(1000);

        builder.HasIndex(x => x.ProductionMaterialIssueId);
        builder.HasIndex(x => x.ProductionExecutionId);
        builder.HasIndex(x => x.ConsumptionDate);

        builder.HasOne(x => x.ProductionMaterialIssue)
            .WithMany(x => x.Consumptions)
            .HasForeignKey(x => x.ProductionMaterialIssueId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ProductionExecution)
            .WithMany(x => x.MaterialConsumptions)
            .HasForeignKey(x => x.ProductionExecutionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
