using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.PaymentReference).IsRequired().HasMaxLength(50);
        builder.Property(x => x.PaymentDate).IsRequired();
        builder.Property(x => x.Status).HasConversion<int>().IsRequired();
        builder.Property(x => x.AmountReceived).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.CurrencyCode).IsRequired().HasMaxLength(10);
        builder.Property(x => x.PaymentMethod).IsRequired().HasMaxLength(50);
        builder.Property(x => x.ExternalTransactionReference).HasMaxLength(100);
        builder.Property(x => x.Notes).HasMaxLength(2000);

        builder.HasIndex(x => x.PaymentReference).IsUnique();
        builder.HasIndex(x => new { x.CustomerId, x.PaymentDate });
        builder.HasIndex(x => x.Status);

        builder.HasMany(x => x.Allocations)
            .WithOne(x => x.Payment)
            .HasForeignKey(x => x.PaymentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
