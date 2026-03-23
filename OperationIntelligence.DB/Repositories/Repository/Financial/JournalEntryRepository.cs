using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class JournalEntryRepository : BaseRepository<JournalEntry>, IJournalEntryRepository
{
    public JournalEntryRepository(OperationIntelligenceDbContext context) : base(context)
    {
    }

    public async Task<JournalEntry?> GetByJournalNumberAsync(string journalNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .FirstOrDefaultAsync(x => x.JournalNumber == journalNumber, cancellationToken);
    }

    public async Task<JournalEntry?> GetWithLinesAsync(Guid journalEntryId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == journalEntryId, cancellationToken);
    }

    public async Task<IReadOnlyList<JournalEntry>> GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.EntryDate >= from && x.EntryDate <= to)
            .OrderByDescending(x => x.EntryDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<JournalEntry>> GetByStatusAsync(JournalEntryStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.Status == status)
            .OrderByDescending(x => x.EntryDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<JournalEntry>> GetBySourceModuleAsync(FinanceSourceModule sourceModule, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.SourceModule == sourceModule)
            .OrderByDescending(x => x.EntryDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<JournalEntry>> GetByFiscalPeriodAsync(Guid fiscalPeriodId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.FiscalPeriodId == fiscalPeriodId)
            .OrderByDescending(x => x.EntryDate)
            .ToListAsync(cancellationToken);
    }
}
