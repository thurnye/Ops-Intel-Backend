namespace OperationIntelligence.DB;

public interface ILoginAttemptRepository : IBaseRepository<LoginAttempt>
{
    Task<int> CountFailedAttemptsByEmailAsync(string email, DateTime fromUtc, CancellationToken cancellationToken = default);
    Task<int> CountFailedAttemptsByUserIdAsync(Guid userId, DateTime fromUtc, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LoginAttempt>> GetRecentByEmailAsync(string email, int take = 10, CancellationToken cancellationToken = default);
}
