using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class RoutingRepository : BaseRepository<Routing>, IRoutingRepository
{
    public RoutingRepository(OperationIntelligenceDbContext context) : base(context)
    {
    }

    public async Task<Routing?> GetWithStepsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(x => x.Product)
            .Include(x => x.Steps.OrderBy(s => s.Sequence))
                .ThenInclude(x => x.WorkCenter)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }

    public async Task<Routing?> GetDefaultByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        return await _dbSet
            .AsNoTracking()
            .Include(x => x.Steps.OrderBy(s => s.Sequence))
            .FirstOrDefaultAsync(x =>
                x.ProductId == productId &&
                x.IsDefault &&
                x.IsActive &&
                !x.IsDeleted &&
                x.EffectiveFrom <= now &&
                (!x.EffectiveTo.HasValue || x.EffectiveTo >= now),
                cancellationToken);
    }

    public async Task<IReadOnlyList<Routing>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(x => x.ProductId == productId && !x.IsDeleted)
            .OrderByDescending(x => x.Version)
            .ToListAsync(cancellationToken);
    }

    public async Task<Routing?> GetActiveVersionAsync(Guid productId, int version, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        return await _dbSet
            .AsNoTracking()
            .Include(x => x.Steps.OrderBy(s => s.Sequence))
            .FirstOrDefaultAsync(x =>
                x.ProductId == productId &&
                x.Version == version &&
                x.IsActive &&
                !x.IsDeleted &&
                x.EffectiveFrom <= now &&
                (!x.EffectiveTo.HasValue || x.EffectiveTo >= now),
                cancellationToken);
    }

    public async Task<bool> RoutingCodeExistsAsync(string routingCode, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(
            x => x.RoutingCode == routingCode &&
                 !x.IsDeleted &&
                 (!excludeId.HasValue || x.Id != excludeId.Value),
            cancellationToken);
    }
}
