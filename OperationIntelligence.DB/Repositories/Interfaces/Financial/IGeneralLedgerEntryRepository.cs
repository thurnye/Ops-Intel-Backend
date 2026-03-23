namespace OperationIntelligence.DB;

public interface IGeneralLedgerEntryRepository : IBaseRepository<GeneralLedgerEntry>
{
    Task<IReadOnlyList<GeneralLedgerEntry>> GetByAccountAndDateRangeAsync(
        Guid accountId,
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<GeneralLedgerEntry>> GetByJournalEntryIdAsync(
        Guid journalEntryId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<GeneralLedgerEntry>> GetByFiscalPeriodAsync(
        Guid fiscalPeriodId,
        CancellationToken cancellationToken = default);

    Task<decimal> GetAccountBalanceAsync(
        Guid accountId,
        DateTime? asOfDate = null,
        CancellationToken cancellationToken = default);
}
