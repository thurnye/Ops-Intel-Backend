using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class ChartOfAccountRepository : BaseRepository<ChartOfAccount>, IChartOfAccountRepository
{
    public ChartOfAccountRepository(OperationIntelligenceDbContext context) : base(context)
    {
    }

    public async Task<ChartOfAccount?> GetByCodeAsync(string accountCode, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .FirstOrDefaultAsync(x => x.AccountCode == accountCode, cancellationToken);
    }

    public async Task<IReadOnlyList<ChartOfAccount>> GetActiveAccountsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.AccountCode)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ChartOfAccount>> GetByTypeAsync(AccountType accountType, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.AccountType == accountType)
            .OrderBy(x => x.AccountCode)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ChartOfAccount>> GetChildrenAsync(Guid parentAccountId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.ParentAccountId == parentAccountId)
            .OrderBy(x => x.AccountCode)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> AccountCodeExistsAsync(string accountCode, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(
            x => x.AccountCode == accountCode && (!excludeId.HasValue || x.Id != excludeId.Value),
            cancellationToken);
    }
}
