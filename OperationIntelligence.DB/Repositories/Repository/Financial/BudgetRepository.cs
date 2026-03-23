using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class BudgetRepository : BaseRepository<Budget>, IBudgetRepository
{
    public BudgetRepository(OperationIntelligenceDbContext context) : base(context)
    {
    }

    public async Task<Budget?> GetByBudgetCodeAsync(string budgetCode, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .FirstOrDefaultAsync(x => x.BudgetCode == budgetCode, cancellationToken);
    }

    public async Task<Budget?> GetWithLinesAsync(Guid budgetId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == budgetId, cancellationToken);
    }

    public async Task<IReadOnlyList<Budget>> GetByFiscalYearAsync(Guid fiscalYearId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.FiscalYearId == fiscalYearId)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Budget>> GetByDepartmentAsync(Guid departmentId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.DepartmentId == departmentId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }
}
