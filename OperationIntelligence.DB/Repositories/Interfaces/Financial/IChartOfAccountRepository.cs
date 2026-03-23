namespace OperationIntelligence.DB;

public interface IChartOfAccountRepository : IBaseRepository<ChartOfAccount>
{
    Task<ChartOfAccount?> GetByCodeAsync(string accountCode, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ChartOfAccount>> GetActiveAccountsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ChartOfAccount>> GetByTypeAsync(AccountType accountType, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ChartOfAccount>> GetChildrenAsync(Guid parentAccountId, CancellationToken cancellationToken = default);
    Task<bool> AccountCodeExistsAsync(string accountCode, Guid? excludeId = null, CancellationToken cancellationToken = default);
}
