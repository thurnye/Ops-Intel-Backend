using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class AccountPayableConfiguration : IEntityTypeConfiguration<AccountPayable>
{
    public void Configure(EntityTypeBuilder<AccountPayable> builder)
    {
        builder.ToTable("AccountsPayable");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.BillDate).IsRequired();
        builder.Property(x => x.DueDate).IsRequired();
        builder.Property(x => x.OriginalAmount).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.AmountPaid).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.OutstandingAmount).HasPrecision(18, 2).IsRequired();

        builder.HasIndex(x => x.VendorBillId).IsUnique();
        builder.HasIndex(x => new { x.VendorId, x.DueDate });
        builder.HasIndex(x => new { x.IsOverdue, x.OutstandingAmount });

        builder.HasOne(x => x.VendorBill)
            .WithOne(x => x.AccountPayable)
            .HasForeignKey<AccountPayable>(x => x.VendorBillId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
