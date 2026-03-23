using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class JournalEntryConfiguration : IEntityTypeConfiguration<JournalEntry>
{
    public void Configure(EntityTypeBuilder<JournalEntry> builder)
    {
        builder.ToTable("JournalEntries");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.JournalNumber).IsRequired().HasMaxLength(50);
        builder.Property(x => x.EntryDate).IsRequired();
        builder.Property(x => x.PostingDate).IsRequired();
        builder.Property(x => x.Status).HasConversion<int>().IsRequired();
        builder.Property(x => x.SourceModule).HasConversion<int>().IsRequired();
        builder.Property(x => x.SourceReferenceType).HasMaxLength(100);
        builder.Property(x => x.Memo).IsRequired().HasMaxLength(2000);
        builder.Property(x => x.ApprovedByUserId).HasMaxLength(128);
        builder.Property(x => x.PostedByUserId).HasMaxLength(128);

        builder.HasIndex(x => x.JournalNumber).IsUnique();
        builder.HasIndex(x => new { x.FiscalPeriodId, x.Status });
        builder.HasIndex(x => new { x.SourceModule, x.SourceReferenceId });

        builder.HasOne(x => x.FiscalPeriod)
            .WithMany(x => x.JournalEntries)
            .HasForeignKey(x => x.FiscalPeriodId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ReversedJournalEntry)
            .WithMany(x => x.ReversalEntries)
            .HasForeignKey(x => x.ReversedJournalEntryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Lines)
            .WithOne(x => x.JournalEntry)
            .HasForeignKey(x => x.JournalEntryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.GeneralLedgerEntries)
            .WithOne(x => x.JournalEntry)
            .HasForeignKey(x => x.JournalEntryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
