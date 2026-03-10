namespace OperationIntelligence.DB;

public interface IEmailVerificationTokenRepository : IBaseRepository<EmailVerificationToken>
{
    Task<EmailVerificationToken?> GetActiveByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EmailVerificationToken>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task InvalidateAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
