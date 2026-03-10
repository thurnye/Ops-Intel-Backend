namespace OperationIntelligence.DB;

public interface IPasswordResetTokenRepository : IBaseRepository<PasswordResetToken>
{
    Task<PasswordResetToken?> GetActiveByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PasswordResetToken>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task InvalidateAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
