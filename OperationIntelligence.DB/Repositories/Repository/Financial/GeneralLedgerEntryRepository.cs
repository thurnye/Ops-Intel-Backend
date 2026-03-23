using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class GeneralLedgerEntryRepository : BaseRepository<GeneralLedgerEntry>, IGeneralLedgerEntryRepository
{
    public GeneralLedgerEntryRepository(OperationIntelligenceDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<GeneralLedgerEntry>> GetByAccountAndDateRangeAsync(Guid accountId, DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.AccountId == accountId && x.PostingDate >= from && x.PostingDate <= to)
            .OrderBy(x => x.PostingDate)
            .ThenBy(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<GeneralLedgerEntry>> GetByJournalEntryIdAsync(Guid journalEntryId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.JournalEntryId == journalEntryId)
            .OrderBy(x => x.PostingDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<GeneralLedgerEntry>> GetByFiscalPeriodAsync(Guid fiscalPeriodId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.FiscalPeriodId == fiscalPeriodId)
            .OrderBy(x => x.PostingDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<decimal> GetAccountBalanceAsync(Guid accountId, DateTime? asOfDate = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsNoTracking().Where(x => x.AccountId == accountId);

        if (asOfDate.HasValue)
        {
            query = query.Where(x => x.PostingDate <= asOfDate.Value);
        }

        return await query.SumAsync(x => x.DebitAmount - x.CreditAmount, cancellationToken);
    }
}
