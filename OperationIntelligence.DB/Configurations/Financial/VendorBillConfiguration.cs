using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class VendorBillConfiguration : IEntityTypeConfiguration<VendorBill>
{
    public void Configure(EntityTypeBuilder<VendorBill> builder)
    {
        builder.ToTable("VendorBills");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.BillNumber).IsRequired().HasMaxLength(50);
        builder.Property(x => x.BillDate).IsRequired();
        builder.Property(x => x.DueDate).IsRequired();
        builder.Property(x => x.Status).HasConversion<int>().IsRequired();
        builder.Property(x => x.Subtotal).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.TaxAmount).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.DiscountAmount).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.TotalAmount).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.AmountPaid).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.OutstandingAmount).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.CurrencyCode).IsRequired().HasMaxLength(10);
        builder.Property(x => x.Notes).HasMaxLength(2000);

        builder.HasIndex(x => x.BillNumber).IsUnique();
        builder.HasIndex(x => new { x.VendorId, x.BillDate });
        builder.HasIndex(x => new { x.Status, x.DueDate });
        builder.HasIndex(x => x.ShipmentId);
        builder.HasIndex(x => x.ProductionBatchId);

        builder.HasMany(x => x.Lines)
            .WithOne(x => x.VendorBill)
            .HasForeignKey(x => x.VendorBillId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.AccountPayable)
            .WithOne(x => x.VendorBill)
            .HasForeignKey<AccountPayable>(x => x.VendorBillId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
