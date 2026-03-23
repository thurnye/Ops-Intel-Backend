using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class FiscalYearRepository : BaseRepository<FiscalYear>, IFiscalYearRepository
{
    public FiscalYearRepository(OperationIntelligenceDbContext context) : base(context)
    {
    }

    public async Task<FiscalYear?> GetByYearCodeAsync(int yearCode, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .FirstOrDefaultAsync(x => x.YearCode == yearCode, cancellationToken);
    }

    public async Task<FiscalYear?> GetCurrentAsync(DateTime asOfDate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .FirstOrDefaultAsync(x => x.StartDate <= asOfDate && x.EndDate >= asOfDate, cancellationToken);
    }

    public async Task<IReadOnlyList<FiscalYear>> GetOpenFiscalYearsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(x => !x.IsClosed)
            .OrderByDescending(x => x.YearCode)
            .ToListAsync(cancellationToken);
    }
}
