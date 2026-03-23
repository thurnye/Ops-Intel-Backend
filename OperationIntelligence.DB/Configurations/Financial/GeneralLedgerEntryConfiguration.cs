using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class GeneralLedgerEntryConfiguration : IEntityTypeConfiguration<GeneralLedgerEntry>
{
    public void Configure(EntityTypeBuilder<GeneralLedgerEntry> builder)
    {
        builder.ToTable("GeneralLedgerEntries");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.PostingDate).IsRequired();
        builder.Property(x => x.DebitAmount).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.CreditAmount).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.CurrencyCode).IsRequired().HasMaxLength(10);
        builder.Property(x => x.ExchangeRate).HasPrecision(18, 6).IsRequired();

        builder.HasIndex(x => new { x.AccountId, x.PostingDate });
        builder.HasIndex(x => new { x.FiscalPeriodId, x.PostingDate });
        builder.HasIndex(x => x.JournalEntryId);
        builder.HasIndex(x => x.JournalLineId);

        builder.HasOne(x => x.JournalEntry)
            .WithMany(x => x.GeneralLedgerEntries)
            .HasForeignKey(x => x.JournalEntryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.JournalLine)
            .WithMany(x => x.GeneralLedgerEntries)
            .HasForeignKey(x => x.JournalLineId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Account)
            .WithMany(x => x.GeneralLedgerEntries)
            .HasForeignKey(x => x.AccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.FiscalPeriod)
            .WithMany(x => x.GeneralLedgerEntries)
            .HasForeignKey(x => x.FiscalPeriodId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CostCenter)
            .WithMany(x => x.GeneralLedgerEntries)
            .HasForeignKey(x => x.CostCenterId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
