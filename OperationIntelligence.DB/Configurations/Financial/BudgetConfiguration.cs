using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OperationIntelligence.DB;

public class BudgetConfiguration : IEntityTypeConfiguration<Budget>
{
    public void Configure(EntityTypeBuilder<Budget> builder)
    {
        builder.ToTable("Budgets");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.BudgetCode).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.TotalBudgetAmount).HasPrecision(18, 2).IsRequired();

        builder.HasIndex(x => x.BudgetCode).IsUnique();
        builder.HasIndex(x => new { x.FiscalYearId, x.DepartmentId });
        builder.HasIndex(x => x.CostCenterId);

        builder.HasOne(x => x.FiscalYear)
            .WithMany(x => x.Budgets)
            .HasForeignKey(x => x.FiscalYearId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CostCenter)
            .WithMany()
            .HasForeignKey(x => x.CostCenterId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Lines)
            .WithOne(x => x.Budget)
            .HasForeignKey(x => x.BudgetId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
