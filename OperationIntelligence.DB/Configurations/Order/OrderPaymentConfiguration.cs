using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;


public class OrderPaymentConfiguration : IEntityTypeConfiguration<OrderPayment>
{
    public void Configure(EntityTypeBuilder<OrderPayment> builder)
    {
        builder.ToTable("OrderPayments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.PaymentReference)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(x => x.PaymentReference)
            .IsUnique();

        builder.Property(x => x.ExternalTransactionId).HasMaxLength(150);
        builder.Property(x => x.ExternalPaymentIntentId).HasMaxLength(150);
        builder.Property(x => x.CurrencyCode).IsRequired().HasMaxLength(10);
        builder.Property(x => x.PayerName).HasMaxLength(200);
        builder.Property(x => x.PayerEmail).HasMaxLength(150);
        builder.Property(x => x.Last4).HasMaxLength(10);
        builder.Property(x => x.AuthorizationCode).HasMaxLength(100);
        builder.Property(x => x.ReceiptNumber).HasMaxLength(100);
        builder.Property(x => x.FailureReason).HasMaxLength(500);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.Property(x => x.RecordedBy).HasMaxLength(150);

        builder.Property(x => x.Amount).HasPrecision(18, 2);
        builder.Property(x => x.FeeAmount).HasPrecision(18, 2);
        builder.Property(x => x.NetAmount).HasPrecision(18, 2);
        builder.Property(x => x.RefundedAmount).HasPrecision(18, 2);

        builder.HasOne(x => x.Order)
            .WithMany(x => x.Payments)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
