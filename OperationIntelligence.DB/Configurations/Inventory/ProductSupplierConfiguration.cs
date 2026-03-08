using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class ProductSupplierConfiguration : IEntityTypeConfiguration<ProductSupplier>
{
    public void Configure(EntityTypeBuilder<ProductSupplier> builder)
    {
        builder.ToTable("ProductSuppliers");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.SupplierProductCode).HasMaxLength(100);
        builder.Property(x => x.SupplierPrice).HasColumnType("decimal(18,2)");

        builder.HasIndex(x => new { x.ProductId, x.SupplierId }).IsUnique();

        builder.HasOne(x => x.Product)
            .WithMany(x => x.ProductSuppliers)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Supplier)
            .WithMany(x => x.ProductSuppliers)
            .HasForeignKey(x => x.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
