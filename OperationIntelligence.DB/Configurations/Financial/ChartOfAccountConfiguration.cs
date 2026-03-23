using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class ChartOfAccountConfiguration : IEntityTypeConfiguration<ChartOfAccount>
{
    public void Configure(EntityTypeBuilder<ChartOfAccount> builder)
    {
        builder.ToTable("ChartOfAccounts");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.AccountCode).IsRequired().HasMaxLength(50);
        builder.Property(x => x.AccountName).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Description).HasMaxLength(1000);
        builder.Property(x => x.AccountType).HasConversion<int>().IsRequired();

        builder.HasIndex(x => x.AccountCode).IsUnique();
        builder.HasIndex(x => new { x.AccountType, x.IsActive });

        builder.HasOne(x => x.ParentAccount)
            .WithMany(x => x.ChildAccounts)
            .HasForeignKey(x => x.ParentAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.JournalLines)
            .WithOne(x => x.Account)
            .HasForeignKey(x => x.AccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.GeneralLedgerEntries)
            .WithOne(x => x.Account)
            .HasForeignKey(x => x.AccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Expenses)
            .WithOne(x => x.ExpenseAccount)
            .HasForeignKey(x => x.ExpenseAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.BudgetLines)
            .WithOne(x => x.Account)
            .HasForeignKey(x => x.AccountId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
