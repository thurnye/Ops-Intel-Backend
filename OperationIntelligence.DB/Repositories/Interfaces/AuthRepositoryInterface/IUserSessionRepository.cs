namespace OperationIntelligence.DB;

public interface IUserSessionRepository : IBaseRepository<UserSession>
{
    Task<IReadOnlyList<UserSession>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<UserSession?> GetByIdWithUserAsync(Guid sessionId, CancellationToken cancellationToken = default);
    Task RevokeSessionAsync(Guid sessionId, CancellationToken cancellationToken = default);
    Task RevokeAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
