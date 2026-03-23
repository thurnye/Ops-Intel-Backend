using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class BudgetLineRepository : BaseRepository<BudgetLine>, IBudgetLineRepository
{
    public BudgetLineRepository(OperationIntelligenceDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<BudgetLine>> GetByBudgetIdAsync(Guid budgetId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.BudgetId == budgetId)
            .OrderBy(x => x.PeriodNumber)
            .ToListAsync(cancellationToken);
    }
}
