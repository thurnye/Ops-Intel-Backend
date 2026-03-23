using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class BudgetLineConfiguration : IEntityTypeConfiguration<BudgetLine>
{
    public void Configure(EntityTypeBuilder<BudgetLine> builder)
    {
        builder.ToTable("BudgetLines");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.PeriodNumber).IsRequired();
        builder.Property(x => x.BudgetAmount).HasPrecision(18, 2).IsRequired();

        builder.HasIndex(x => new { x.BudgetId, x.AccountId, x.PeriodNumber }).IsUnique();

        builder.HasOne(x => x.Budget)
            .WithMany(x => x.Lines)
            .HasForeignKey(x => x.BudgetId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Account)
            .WithMany(x => x.BudgetLines)
            .HasForeignKey(x => x.AccountId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
