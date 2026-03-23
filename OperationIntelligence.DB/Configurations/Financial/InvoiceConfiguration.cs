using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("Invoices");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.InvoiceNumber).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Status).HasConversion<int>().IsRequired();
        builder.Property(x => x.InvoiceDate).IsRequired();
        builder.Property(x => x.DueDate).IsRequired();
        builder.Property(x => x.Subtotal).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.TaxAmount).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.DiscountAmount).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.TotalAmount).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.AmountPaid).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.OutstandingAmount).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.CurrencyCode).IsRequired().HasMaxLength(10);
        builder.Property(x => x.Notes).HasMaxLength(2000);

        builder.HasIndex(x => x.InvoiceNumber).IsUnique();
        builder.HasIndex(x => new { x.CustomerId, x.InvoiceDate });
        builder.HasIndex(x => new { x.Status, x.DueDate });
        builder.HasIndex(x => x.OrderId);

        builder.HasMany(x => x.Lines)
            .WithOne(x => x.Invoice)
            .HasForeignKey(x => x.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.PaymentAllocations)
            .WithOne(x => x.Invoice)
            .HasForeignKey(x => x.InvoiceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.AccountReceivable)
            .WithOne(x => x.Invoice)
            .HasForeignKey<AccountReceivable>(x => x.InvoiceId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
