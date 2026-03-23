using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class JournalLineConfiguration : IEntityTypeConfiguration<JournalLine>
{
    public void Configure(EntityTypeBuilder<JournalLine> builder)
    {
        builder.ToTable("JournalLines");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.LineNumber).IsRequired();
        builder.Property(x => x.Description).IsRequired().HasMaxLength(1000);
        builder.Property(x => x.DebitAmount).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.CreditAmount).HasPrecision(18, 2).IsRequired();
        builder.Property(x => x.CurrencyCode).IsRequired().HasMaxLength(10);
        builder.Property(x => x.ExchangeRate).HasPrecision(18, 6).IsRequired();

        builder.HasIndex(x => new { x.JournalEntryId, x.LineNumber }).IsUnique();
        builder.HasIndex(x => x.AccountId);
        builder.HasIndex(x => x.CostCenterId);

        builder.HasOne(x => x.JournalEntry)
            .WithMany(x => x.Lines)
            .HasForeignKey(x => x.JournalEntryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Account)
            .WithMany(x => x.JournalLines)
            .HasForeignKey(x => x.AccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CostCenter)
            .WithMany(x => x.JournalLines)
            .HasForeignKey(x => x.CostCenterId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.GeneralLedgerEntries)
            .WithOne(x => x.JournalLine)
            .HasForeignKey(x => x.JournalLineId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
