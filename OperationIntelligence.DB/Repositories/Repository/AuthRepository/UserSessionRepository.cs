using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class UserSessionRepository : BaseRepository<UserSession>, IUserSessionRepository
{
    public UserSessionRepository(OperationIntelligenceDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<UserSession>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.UserSessions
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.RevokedAtUtc == null)
            .OrderByDescending(x => x.LastSeenAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<UserSession?> GetByIdWithUserAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        return await _context.UserSessions
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == sessionId, cancellationToken);
    }

    public async Task RevokeSessionAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        var session = await _context.UserSessions
            .FirstOrDefaultAsync(x => x.Id == sessionId, cancellationToken);

        if (session is null)
            return;

        session.RevokedAtUtc = DateTime.UtcNow;
    }

    public async Task RevokeAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var sessions = await _context.UserSessions
            .Where(x => x.UserId == userId && x.RevokedAtUtc == null)
            .ToListAsync(cancellationToken);

        var utcNow = DateTime.UtcNow;

        foreach (var session in sessions)
        {
            session.RevokedAtUtc = utcNow;
        }
    }
}
