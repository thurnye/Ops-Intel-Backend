using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
{
    public void Configure(EntityTypeBuilder<Expense> builder)
    {
        builder.ToTable("Expenses");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ExpenseNumber).IsRequired().HasMaxLength(50);
        builder.Property(x => x.ExpenseDate).IsRequired();
        builder.Property(x => x.Category).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Description).IsRequired().HasMaxLength(2000);
        builder.Property(x => x.Amount).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.TaxAmount).HasPrecision(18, 2).IsRequired();

        builder.HasIndex(x => x.ExpenseNumber).IsUnique();
        builder.HasIndex(x => new { x.ExpenseDate, x.Category });
        builder.HasIndex(x => x.ExpenseAccountId);
        builder.HasIndex(x => x.CostCenterId);
        builder.HasIndex(x => x.VendorId);
        builder.HasIndex(x => x.RelatedJournalEntryId);

        builder.HasOne(x => x.ExpenseAccount)
            .WithMany(x => x.Expenses)
            .HasForeignKey(x => x.ExpenseAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CostCenter)
            .WithMany(x => x.Expenses)
            .HasForeignKey(x => x.CostCenterId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.RelatedJournalEntry)
            .WithMany()
            .HasForeignKey(x => x.RelatedJournalEntryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
