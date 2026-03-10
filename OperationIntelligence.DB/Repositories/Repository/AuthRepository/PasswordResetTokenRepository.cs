using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class PasswordResetTokenRepository : BaseRepository<PasswordResetToken>, IPasswordResetTokenRepository
{
    public PasswordResetTokenRepository(OperationIntelligenceDbContext context) : base(context)
    {
    }

    public async Task<PasswordResetToken?> GetActiveByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default)
    {
        var utcNow = DateTime.UtcNow;

        return await _context.PasswordResetTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(
                x => x.TokenHash == tokenHash &&
                     x.UsedAtUtc == null &&
                     x.ExpiresAtUtc > utcNow,
                cancellationToken);
    }

    public async Task<IReadOnlyList<PasswordResetToken>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var utcNow = DateTime.UtcNow;

        return await _context.PasswordResetTokens
            .Where(x => x.UserId == userId &&
                        x.UsedAtUtc == null &&
                        x.ExpiresAtUtc > utcNow)
            .ToListAsync(cancellationToken);
    }

    public async Task InvalidateAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var utcNow = DateTime.UtcNow;

        var tokens = await _context.PasswordResetTokens
            .Where(x => x.UserId == userId &&
                        x.UsedAtUtc == null &&
                        x.ExpiresAtUtc > utcNow)
            .ToListAsync(cancellationToken);

        foreach (var token in tokens)
        {
            token.UsedAtUtc = utcNow;
        }
    }
}
