namespace OperationIntelligence.DB;

public interface IRoleRepository : IBaseRepository<Role>
{
    Task<Role?> GetByNameAsync(string normalizedName, CancellationToken cancellationToken = default);
    Task<Role?> GetWithPermissionsAsync(Guid roleId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Role>> GetSystemRolesAsync(CancellationToken cancellationToken = default);
    Task<bool> RoleExistsAsync(string normalizedName, CancellationToken cancellationToken = default);
}
