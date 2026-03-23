namespace OperationIntelligence.DB;

public interface ICostCenterRepository : IBaseRepository<CostCenter>
{
    Task<CostCenter?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CostCenter>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<bool> CodeExistsAsync(string code, Guid? excludeId = null, CancellationToken cancellationToken = default);
}
