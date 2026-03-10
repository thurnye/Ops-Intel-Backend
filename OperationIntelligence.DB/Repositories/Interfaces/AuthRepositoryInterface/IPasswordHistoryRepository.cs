namespace OperationIntelligence.DB;

public interface IPasswordHistoryRepository : IBaseRepository<PasswordHistory>
{
    Task<IReadOnlyList<PasswordHistory>> GetRecentByUserIdAsync(Guid userId, int take = 5, CancellationToken cancellationToken = default);
}
