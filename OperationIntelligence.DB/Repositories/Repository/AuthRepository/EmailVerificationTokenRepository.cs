using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class EmailVerificationTokenRepository : BaseRepository<EmailVerificationToken>, IEmailVerificationTokenRepository
{
    public EmailVerificationTokenRepository(OperationIntelligenceDbContext context) : base(context)
    {
    }

    public async Task<EmailVerificationToken?> GetActiveByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default)
    {
        var utcNow = DateTime.UtcNow;

        return await _context.EmailVerificationTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(
                x => x.TokenHash == tokenHash &&
                     x.VerifiedAtUtc == null &&
                     x.ExpiresAtUtc > utcNow,
                cancellationToken);
    }

    public async Task<IReadOnlyList<EmailVerificationToken>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var utcNow = DateTime.UtcNow;

        return await _context.EmailVerificationTokens
            .Where(x => x.UserId == userId &&
                        x.VerifiedAtUtc == null &&
                        x.ExpiresAtUtc > utcNow)
            .ToListAsync(cancellationToken);
    }

    public async Task InvalidateAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var utcNow = DateTime.UtcNow;

        var tokens = await _context.EmailVerificationTokens
            .Where(x => x.UserId == userId &&
                        x.VerifiedAtUtc == null &&
                        x.ExpiresAtUtc > utcNow)
            .ToListAsync(cancellationToken);

        foreach (var token in tokens)
        {
            token.VerifiedAtUtc = utcNow;
        }
    }
}
