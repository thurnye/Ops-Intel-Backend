namespace OperationIntelligence.DB;

public interface IFiscalPeriodRepository : IBaseRepository<FiscalPeriod>
{
    Task<FiscalPeriod?> GetByFiscalYearAndPeriodAsync(Guid fiscalYearId, int periodNumber, CancellationToken cancellationToken = default);
    Task<FiscalPeriod?> GetCurrentOpenPeriodAsync(CancellationToken cancellationToken = default);
    Task<FiscalPeriod?> GetByDateAsync(DateTime date, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FiscalPeriod>> GetByFiscalYearAsync(Guid fiscalYearId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FiscalPeriod>> GetByStatusAsync(FiscalPeriodStatus status, CancellationToken cancellationToken = default);
    Task<bool> IsPeriodOpenAsync(DateTime date, CancellationToken cancellationToken = default);
}
