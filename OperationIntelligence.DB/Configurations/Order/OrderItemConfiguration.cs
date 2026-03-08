using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;


public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ProductNameSnapshot)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.ProductSkuSnapshot)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ProductDescriptionSnapshot)
            .HasMaxLength(1000);

        builder.Property(x => x.QuantityOrdered).HasPrecision(18, 2);
        builder.Property(x => x.QuantityAllocated).HasPrecision(18, 2);
        builder.Property(x => x.QuantityShipped).HasPrecision(18, 2);
        builder.Property(x => x.QuantityDelivered).HasPrecision(18, 2);
        builder.Property(x => x.QuantityCancelled).HasPrecision(18, 2);

        builder.Property(x => x.UnitPrice).HasPrecision(18, 2);
        builder.Property(x => x.DiscountAmount).HasPrecision(18, 2);
        builder.Property(x => x.TaxAmount).HasPrecision(18, 2);
        builder.Property(x => x.LineTotal).HasPrecision(18, 2);

        builder.Property(x => x.Remarks).HasMaxLength(500);

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