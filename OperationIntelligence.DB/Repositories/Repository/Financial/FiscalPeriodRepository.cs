using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class FiscalPeriodRepository : BaseRepository<FiscalPeriod>, IFiscalPeriodRepository
{
    public FiscalPeriodRepository(OperationIntelligenceDbContext context) : base(context)
    {
    }

    public async Task<FiscalPeriod?> GetByFiscalYearAndPeriodAsync(Guid fiscalYearId, int periodNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .FirstOrDefaultAsync(x => x.FiscalYearId == fiscalYearId && x.PeriodNumber == periodNumber, cancellationToken);
    }

    public async Task<FiscalPeriod?> GetCurrentOpenPeriodAsync(CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;

        return await _dbSet.AsNoTracking()
            .FirstOrDefaultAsync(x => x.StartDate <= today && x.EndDate >= today && x.Status == FiscalPeriodStatus.Open, cancellationToken);
    }

    public async Task<FiscalPeriod?> GetByDateAsync(DateTime date, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .FirstOrDefaultAsync(x => x.StartDate <= date && x.EndDate >= date, cancellationToken);
    }

    public async Task<IReadOnlyList<FiscalPeriod>> GetByFiscalYearAsync(Guid fiscalYearId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.FiscalYearId == fiscalYearId)
            .OrderBy(x => x.PeriodNumber)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<FiscalPeriod>> GetByStatusAsync(FiscalPeriodStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.Status == status)
            .OrderBy(x => x.StartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsPeriodOpenAsync(DateTime date, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .AnyAsync(x => x.StartDate <= date && x.EndDate >= date && x.Status == FiscalPeriodStatus.Open, cancellationToken);
    }
}
