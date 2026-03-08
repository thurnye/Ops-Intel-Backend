using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.OrderNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(x => x.OrderNumber)
            .IsUnique();

        builder.Property(x => x.CustomerName).HasMaxLength(200);
        builder.Property(x => x.CustomerEmail).HasMaxLength(150);
        builder.Property(x => x.CustomerPhone).HasMaxLength(50);
        builder.Property(x => x.CurrencyCode).IsRequired().HasMaxLength(10);
        builder.Property(x => x.ReferenceNumber).HasMaxLength(100);
        builder.Property(x => x.CustomerPurchaseOrderNumber).HasMaxLength(100);
        builder.Property(x => x.SalesPerson).HasMaxLength(150);
        builder.Property(x => x.ApprovedBy).HasMaxLength(150);
        builder.Property(x => x.CancellationReason).HasMaxLength(500);

        builder.Property(x => x.SubtotalAmount).HasPrecision(18, 2);
        builder.Property(x => x.DiscountAmount).HasPrecision(18, 2);
        builder.Property(x => x.TaxAmount).HasPrecision(18, 2);
        builder.Property(x => x.ShippingAmount).HasPrecision(18, 2);
        builder.Property(x => x.TotalAmount).HasPrecision(18, 2);

        builder.HasOne(x => x.Warehouse)
            .WithMany()
            .HasForeignKey(x => x.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Items)
            .WithOne(x => x.Order)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Images)
            .WithOne(x => x.Order)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.StatusHistory)
            .WithOne(x => x.Order)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.OrderNotes)
            .WithOne(x => x.Order)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
