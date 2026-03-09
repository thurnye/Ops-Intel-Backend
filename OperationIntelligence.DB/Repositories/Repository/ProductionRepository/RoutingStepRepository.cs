using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class RoutingStepRepository : BaseRepository<RoutingStep>, IRoutingStepRepository
{
    public RoutingStepRepository(OperationIntelligenceDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<RoutingStep>> GetByRoutingIdAsync(Guid routingId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(x => x.WorkCenter)
            .Where(x => x.RoutingId == routingId && !x.IsDeleted)
            .OrderBy(x => x.Sequence)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<RoutingStep>> GetByWorkCenterIdAsync(Guid workCenterId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(x => x.WorkCenterId == workCenterId && !x.IsDeleted)
            .OrderBy(x => x.Sequence)
            .ToListAsync(cancellationToken);
    }

    public async Task<RoutingStep?> GetByRoutingAndSequenceAsync(Guid routingId, int sequence, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(x => x.WorkCenter)
            .FirstOrDefaultAsync(
                x => x.RoutingId == routingId &&
                     x.Sequence == sequence &&
                     !x.IsDeleted,
                cancellationToken);
    }
}
