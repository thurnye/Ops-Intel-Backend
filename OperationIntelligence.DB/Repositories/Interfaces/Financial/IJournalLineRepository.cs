namespace OperationIntelligence.DB;

public interface IJournalLineRepository : IBaseRepository<JournalLine>
{
    Task<IReadOnlyList<JournalLine>> GetByJournalEntryIdAsync(Guid journalEntryId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<JournalLine>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalDebitsAsync(Guid journalEntryId, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalCreditsAsync(Guid journalEntryId, CancellationToken cancellationToken = default);
}
