namespace OperationIntelligence.DB;

public interface IPlatformUserRepository : IBaseRepository<PlatformUser>
{
    Task<PlatformUser?> GetByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default);
    Task<PlatformUser?> GetByUserNameAsync(string normalizedUserName, CancellationToken cancellationToken = default);
    Task<PlatformUser?> GetByEmailOrUserNameAsync(string normalizedValue, CancellationToken cancellationToken = default);

    Task<PlatformUser?> GetWithProfileAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<PlatformUser?> GetWithRolesAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<PlatformUser?> GetFullAuthUserAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<bool> EmailExistsAsync(string normalizedEmail, CancellationToken cancellationToken = default);
    Task<bool> UserNameExistsAsync(string normalizedUserName, CancellationToken cancellationToken = default);

    Task AddUserWithProfileAsync(
        PlatformUser user,
        PlatformUserProfile profile,
        CancellationToken cancellationToken = default);
}
