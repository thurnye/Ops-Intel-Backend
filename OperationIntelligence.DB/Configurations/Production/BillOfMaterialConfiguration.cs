using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class BillOfMaterialConfiguration : IEntityTypeConfiguration<BillOfMaterial>
{
    public void Configure(EntityTypeBuilder<BillOfMaterial> builder)
    {
        builder.ToTable("BillsOfMaterial");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.BomCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.BaseQuantity)
            .HasPrecision(18, 4);

        builder.Property(x => x.Version)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);

        builder.Property(x => x.IsDefault)
            .HasDefaultValue(false);

        builder.Property(x => x.Notes)
            .HasMaxLength(1000);

        builder.HasIndex(x => x.BomCode)
            .IsUnique();

        builder.HasIndex(x => new { x.ProductId, x.Version })
            .IsUnique();

        builder.HasIndex(x => x.ProductId);

        builder.HasIndex(x => x.UnitOfMeasureId);

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
