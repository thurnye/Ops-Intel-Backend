namespace OperationIntelligence.DB;

public interface IFiscalYearRepository : IBaseRepository<FiscalYear>
{
    Task<FiscalYear?> GetByYearCodeAsync(int yearCode, CancellationToken cancellationToken = default);
    Task<FiscalYear?> GetCurrentAsync(DateTime asOfDate, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FiscalYear>> GetOpenFiscalYearsAsync(CancellationToken cancellationToken = default);
}
