using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class JournalLineRepository : BaseRepository<JournalLine>, IJournalLineRepository
{
    public JournalLineRepository(OperationIntelligenceDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<JournalLine>> GetByJournalEntryIdAsync(Guid journalEntryId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.JournalEntryId == journalEntryId)
            .OrderBy(x => x.LineNumber)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<JournalLine>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.AccountId == accountId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<decimal> GetTotalDebitsAsync(Guid journalEntryId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.JournalEntryId == journalEntryId)
            .SumAsync(x => x.DebitAmount, cancellationToken);
    }

    public async Task<decimal> GetTotalCreditsAsync(Guid journalEntryId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.JournalEntryId == journalEntryId)
            .SumAsync(x => x.CreditAmount, cancellationToken);
    }
}
