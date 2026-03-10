using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class RefreshTokenRepository : BaseRepository<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(OperationIntelligenceDbContext context) : base(context)
    {
    }

    public async Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default)
    {
        return await _context.RefreshTokens
            .Include(x => x.User)
                .ThenInclude(x => x.Profile)
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken);
    }

    public async Task<RefreshToken?> GetActiveByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default)
    {
        var utcNow = DateTime.UtcNow;

        return await _context.RefreshTokens
            .Include(x => x.User)
                .ThenInclude(x => x.Profile)
            .FirstOrDefaultAsync(
                x => x.TokenHash == tokenHash &&
                     x.RevokedAtUtc == null &&
                     x.ExpiresAtUtc > utcNow,
                cancellationToken);
    }

    public async Task<IReadOnlyList<RefreshToken>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var utcNow = DateTime.UtcNow;

        return await _context.RefreshTokens
            .Where(x => x.UserId == userId &&
                        x.RevokedAtUtc == null &&
                        x.ExpiresAtUtc > utcNow)
            .ToListAsync(cancellationToken);
    }

    public async Task RevokeAllByUserIdAsync(Guid userId, string? revokedByIp = null, CancellationToken cancellationToken = default)
    {
        var utcNow = DateTime.UtcNow;

        var tokens = await _context.RefreshTokens
            .Where(x => x.UserId == userId &&
                        x.RevokedAtUtc == null &&
                        x.ExpiresAtUtc > utcNow)
            .ToListAsync(cancellationToken);

        foreach (var token in tokens)
        {
            token.RevokedAtUtc = utcNow;
            token.RevokedByIp = revokedByIp;
        }
    }
}
