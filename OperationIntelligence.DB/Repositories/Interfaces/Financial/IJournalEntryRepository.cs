namespace OperationIntelligence.DB;

public interface IJournalEntryRepository : IBaseRepository<JournalEntry>
{
    Task<JournalEntry?> GetByJournalNumberAsync(string journalNumber, CancellationToken cancellationToken = default);
    Task<JournalEntry?> GetWithLinesAsync(Guid journalEntryId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<JournalEntry>> GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<JournalEntry>> GetByStatusAsync(JournalEntryStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<JournalEntry>> GetBySourceModuleAsync(FinanceSourceModule sourceModule, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<JournalEntry>> GetByFiscalPeriodAsync(Guid fiscalPeriodId, CancellationToken cancellationToken = default);
}
