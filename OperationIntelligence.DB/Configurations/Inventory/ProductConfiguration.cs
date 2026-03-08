using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace OperationIntelligence.DB;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .HasMaxLength(2000);

        builder.Property(x => x.SKU)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(x => x.SKU).IsUnique();

        builder.Property(x => x.Barcode)
            .HasMaxLength(100);

        builder.HasIndex(x => x.Barcode)
            .IsUnique()
            .HasFilter("[Barcode] IS NOT NULL");

        builder.Property(x => x.CostPrice)
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.SellingPrice)
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.TaxRate)
            .HasColumnType("decimal(5,2)");

        builder.Property(x => x.ReorderLevel)
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.ReorderQuantity)
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.Weight).HasColumnType("decimal(18,3)");
        builder.Property(x => x.Length).HasColumnType("decimal(18,2)");
        builder.Property(x => x.Width).HasColumnType("decimal(18,2)");
        builder.Property(x => x.Height).HasColumnType("decimal(18,2)");

        builder.Property(x => x.ThumbnailImageUrl)
            .HasMaxLength(1000);

        builder.HasOne(x => x.Category)
            .WithMany(x => x.Products)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Brand)
            .WithMany(x => x.Products)
            .HasForeignKey(x => x.BrandId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.UnitOfMeasure)
            .WithMany(x => x.Products)
            .HasForeignKey(x => x.UnitOfMeasureId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}