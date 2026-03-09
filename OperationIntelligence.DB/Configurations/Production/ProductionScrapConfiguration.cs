using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class ProductionScrapConfiguration : IEntityTypeConfiguration<ProductionScrap>
{
    public void Configure(EntityTypeBuilder<ProductionScrap> builder)
    {
        builder.ToTable("ProductionScraps");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ScrapQuantity)
            .HasPrecision(18, 4);

        builder.Property(x => x.Reason)
            .IsRequired();

        builder.Property(x => x.ReasonDescription)
            .HasMaxLength(1000);

        builder.Property(x => x.IsReworkable)
            .HasDefaultValue(false);

        builder.Property(x => x.Notes)
            .HasMaxLength(1000);

        builder.HasIndex(x => x.ProductionOrderId);
        builder.HasIndex(x => x.ProductionExecutionId);
        builder.HasIndex(x => x.ProductId);
        builder.HasIndex(x => x.UnitOfMeasureId);
        builder.HasIndex(x => x.Reason);
        builder.HasIndex(x => x.ScrapDate);

        builder.HasOne(x => x.ProductionOrder)
            .WithMany(x => x.Scraps)
            .HasForeignKey(x => x.ProductionOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ProductionExecution)
            .WithMany(x => x.Scraps)
            .HasForeignKey(x => x.ProductionExecutionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.UnitOfMeasure)
            .WithMany()
            .HasForeignKey(x => x.UnitOfMeasureId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
