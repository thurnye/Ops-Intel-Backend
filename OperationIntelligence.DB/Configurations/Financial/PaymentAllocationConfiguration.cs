using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class PaymentAllocationConfiguration : IEntityTypeConfiguration<PaymentAllocation>
{
    public void Configure(EntityTypeBuilder<PaymentAllocation> builder)
    {
        builder.ToTable("PaymentAllocations");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.AmountApplied).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.AllocationDate).IsRequired();

        builder.HasIndex(x => new { x.PaymentId, x.InvoiceId });

        builder.HasOne(x => x.Payment)
            .WithMany(x => x.Allocations)
            .HasForeignKey(x => x.PaymentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Invoice)
            .WithMany(x => x.PaymentAllocations)
            .HasForeignKey(x => x.InvoiceId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
