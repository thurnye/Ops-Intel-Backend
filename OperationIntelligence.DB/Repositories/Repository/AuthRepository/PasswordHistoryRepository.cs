using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class PasswordHistoryRepository : BaseRepository<PasswordHistory>, IPasswordHistoryRepository
{
    public PasswordHistoryRepository(OperationIntelligenceDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<PasswordHistory>> GetRecentByUserIdAsync(Guid userId, int take = 5, CancellationToken cancellationToken = default)
    {
        return await _context.PasswordHistories
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Take(take)
            .ToListAsync(cancellationToken);
    }
}
