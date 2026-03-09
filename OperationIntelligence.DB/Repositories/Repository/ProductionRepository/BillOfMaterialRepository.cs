using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class BillOfMaterialRepository : BaseRepository<BillOfMaterial>, IBillOfMaterialRepository
{
    public BillOfMaterialRepository(OperationIntelligenceDbContext context) : base(context)
    {
    }

    public async Task<BillOfMaterial?> GetWithItemsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(x => x.Product)
            .Include(x => x.UnitOfMeasure)
            .Include(x => x.Items)
                .ThenInclude(x => x.MaterialProduct)
            .Include(x => x.Items)
                .ThenInclude(x => x.UnitOfMeasure)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }

    public async Task<BillOfMaterial?> GetDefaultByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        return await _dbSet
            .AsNoTracking()
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x =>
                x.ProductId == productId &&
                x.IsDefault &&
                x.IsActive &&
                !x.IsDeleted &&
                x.EffectiveFrom <= now &&
                (!x.EffectiveTo.HasValue || x.EffectiveTo >= now),
                cancellationToken);
    }

    public async Task<IReadOnlyList<BillOfMaterial>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(x => x.ProductId == productId && !x.IsDeleted)
            .OrderByDescending(x => x.Version)
            .ToListAsync(cancellationToken);
    }

    public async Task<BillOfMaterial?> GetActiveVersionAsync(Guid productId, int version, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        return await _dbSet
            .AsNoTracking()
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x =>
                x.ProductId == productId &&
                x.Version == version &&
                x.IsActive &&
                !x.IsDeleted &&
                x.EffectiveFrom <= now &&
                (!x.EffectiveTo.HasValue || x.EffectiveTo >= now),
                cancellationToken);
    }

    public async Task<bool> BomCodeExistsAsync(string bomCode, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(
            x => x.BomCode == bomCode &&
                 !x.IsDeleted &&
                 (!excludeId.HasValue || x.Id != excludeId.Value),
            cancellationToken);
    }
}
