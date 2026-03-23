using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class AccountReceivableConfiguration : IEntityTypeConfiguration<AccountReceivable>
{
    public void Configure(EntityTypeBuilder<AccountReceivable> builder)
    {
        builder.ToTable("AccountsReceivable");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.InvoiceDate).IsRequired();
        builder.Property(x => x.DueDate).IsRequired();
        builder.Property(x => x.OriginalAmount).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.AmountPaid).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.OutstandingAmount).HasPrecision(18, 2).IsRequired();

        builder.HasIndex(x => x.InvoiceId).IsUnique();
        builder.HasIndex(x => new { x.CustomerId, x.DueDate });
        builder.HasIndex(x => new { x.IsOverdue, x.OutstandingAmount });

        builder.HasOne(x => x.Invoice)
            .WithOne(x => x.AccountReceivable)
            .HasForeignKey<AccountReceivable>(x => x.InvoiceId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
