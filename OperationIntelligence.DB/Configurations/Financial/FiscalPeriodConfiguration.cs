using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class FiscalPeriodConfiguration : IEntityTypeConfiguration<FiscalPeriod>
{
    public void Configure(EntityTypeBuilder<FiscalPeriod> builder)
    {
        builder.ToTable("FiscalPeriods");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.PeriodNumber).IsRequired();
        builder.Property(x => x.StartDate).IsRequired();
        builder.Property(x => x.EndDate).IsRequired();
        builder.Property(x => x.Status).HasConversion<int>().IsRequired();

        builder.HasIndex(x => new { x.FiscalYearId, x.PeriodNumber }).IsUnique();
        builder.HasIndex(x => new { x.StartDate, x.EndDate });

        builder.HasMany(x => x.JournalEntries)
            .WithOne(x => x.FiscalPeriod)
            .HasForeignKey(x => x.FiscalPeriodId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.GeneralLedgerEntries)
            .WithOne(x => x.FiscalPeriod)
            .HasForeignKey(x => x.FiscalPeriodId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
