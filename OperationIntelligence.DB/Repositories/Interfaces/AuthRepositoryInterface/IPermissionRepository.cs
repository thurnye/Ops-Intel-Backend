namespace OperationIntelligence.DB;

public interface IPermissionRepository : IBaseRepository<Permission>
{
    Task<Permission?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Permission>> GetByCodesAsync(IEnumerable<string> codes, CancellationToken cancellationToken = default);
    Task<bool> PermissionExistsAsync(string code, CancellationToken cancellationToken = default);
}
