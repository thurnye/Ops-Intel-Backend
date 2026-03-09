using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class BillOfMaterialItemConfiguration : IEntityTypeConfiguration<BillOfMaterialItem>
{
    public void Configure(EntityTypeBuilder<BillOfMaterialItem> builder)
    {
        builder.ToTable("BillOfMaterialItems");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.QuantityRequired)
            .HasPrecision(18, 4);

        builder.Property(x => x.ScrapFactorPercent)
            .HasPrecision(9, 4);

        builder.Property(x => x.YieldFactorPercent)
            .HasPrecision(9, 4)
            .HasDefaultValue(100m);

        builder.Property(x => x.IsOptional)
            .HasDefaultValue(false);

        builder.Property(x => x.IsBackflush)
            .HasDefaultValue(false);

        builder.Property(x => x.Notes)
            .HasMaxLength(1000);

        builder.HasIndex(x => x.BillOfMaterialId);

        builder.HasIndex(x => x.MaterialProductId);

        builder.HasIndex(x => x.UnitOfMeasureId);

        builder.HasIndex(x => new { x.BillOfMaterialId, x.Sequence })
            .IsUnique();

        builder.HasOne(x => x.BillOfMaterial)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.BillOfMaterialId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.MaterialProduct)
            .WithMany()
            .HasForeignKey(x => x.MaterialProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.UnitOfMeasure)
            .WithMany()
            .HasForeignKey(x => x.UnitOfMeasureId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
