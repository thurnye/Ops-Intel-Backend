using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class ProductionQualityCheckConfiguration : IEntityTypeConfiguration<ProductionQualityCheck>
{
    public void Configure(EntityTypeBuilder<ProductionQualityCheck> builder)
    {
        builder.ToTable("ProductionQualityChecks");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CheckType)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.ReferenceStandard)
            .HasMaxLength(200);

        builder.Property(x => x.Findings)
            .HasMaxLength(2000);

        builder.Property(x => x.CorrectiveAction)
            .HasMaxLength(2000);

        builder.Property(x => x.RequiresRework)
            .HasDefaultValue(false);

        builder.Property(x => x.Notes)
            .HasMaxLength(1000);

        builder.Property(x => x.CheckedByUserId)
            .IsRequired();

        builder.HasIndex(x => x.ProductionOrderId);
        builder.HasIndex(x => x.ProductionExecutionId);
        builder.HasIndex(x => x.CheckedByUserId);
        builder.HasIndex(x => x.CheckType);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.CheckDate);

        builder.HasOne(x => x.ProductionOrder)
            .WithMany(x => x.QualityChecks)
            .HasForeignKey(x => x.ProductionOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ProductionExecution)
            .WithMany(x => x.QualityChecks)
            .HasForeignKey(x => x.ProductionExecutionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CheckedByUser)
            .WithMany()
            .HasForeignKey(x => x.CheckedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
