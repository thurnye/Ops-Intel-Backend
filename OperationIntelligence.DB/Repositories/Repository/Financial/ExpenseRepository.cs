using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class ExpenseRepository : BaseRepository<Expense>, IExpenseRepository
{
    public ExpenseRepository(OperationIntelligenceDbContext context) : base(context)
    {
    }

    public async Task<Expense?> GetByExpenseNumberAsync(string expenseNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .FirstOrDefaultAsync(x => x.ExpenseNumber == expenseNumber, cancellationToken);
    }

    public async Task<IReadOnlyList<Expense>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.Category == category)
            .OrderByDescending(x => x.ExpenseDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Expense>> GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.ExpenseDate >= from && x.ExpenseDate <= to)
            .OrderByDescending(x => x.ExpenseDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Expense>> GetByCostCenterAsync(Guid costCenterId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.CostCenterId == costCenterId)
            .OrderByDescending(x => x.ExpenseDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<decimal> GetTotalByCategoryAsync(string category, DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.Category == category && x.ExpenseDate >= from && x.ExpenseDate <= to)
            .SumAsync(x => x.Amount, cancellationToken);
    }
}
