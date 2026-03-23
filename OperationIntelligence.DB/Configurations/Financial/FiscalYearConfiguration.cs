using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class FiscalYearConfiguration : IEntityTypeConfiguration<FiscalYear>
{
    public void Configure(EntityTypeBuilder<FiscalYear> builder)
    {
        builder.ToTable("FiscalYears");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.YearCode).IsRequired();
        builder.Property(x => x.Name).IsRequired().HasMaxLength(50);
        builder.Property(x => x.StartDate).IsRequired();
        builder.Property(x => x.EndDate).IsRequired();

        builder.HasIndex(x => x.YearCode).IsUnique();
        builder.HasIndex(x => x.Name).IsUnique();

        builder.HasMany(x => x.Periods)
            .WithOne(x => x.FiscalYear)
            .HasForeignKey(x => x.FiscalYearId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Budgets)
            .WithOne(x => x.FiscalYear)
            .HasForeignKey(x => x.FiscalYearId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
