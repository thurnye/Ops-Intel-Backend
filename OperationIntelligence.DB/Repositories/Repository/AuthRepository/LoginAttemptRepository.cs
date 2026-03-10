using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class LoginAttemptRepository : BaseRepository<LoginAttempt>, ILoginAttemptRepository
{
    public LoginAttemptRepository(OperationIntelligenceDbContext context) : base(context)
    {
    }

    public async Task<int> CountFailedAttemptsByEmailAsync(string email, DateTime fromUtc, CancellationToken cancellationToken = default)
    {
        return await _context.LoginAttempts.CountAsync(
            x => x.Email == email &&
                 !x.IsSuccessful &&
                 x.CreatedAtUtc >= fromUtc,
            cancellationToken);
    }

    public async Task<int> CountFailedAttemptsByUserIdAsync(Guid userId, DateTime fromUtc, CancellationToken cancellationToken = default)
    {
        return await _context.LoginAttempts.CountAsync(
            x => x.UserId == userId &&
                 !x.IsSuccessful &&
                 x.CreatedAtUtc >= fromUtc,
            cancellationToken);
    }

    public async Task<IReadOnlyList<LoginAttempt>> GetRecentByEmailAsync(string email, int take = 10, CancellationToken cancellationToken = default)
    {
        return await _context.LoginAttempts
            .AsNoTracking()
            .Where(x => x.Email == email)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Take(take)
            .ToListAsync(cancellationToken);
    }
}
