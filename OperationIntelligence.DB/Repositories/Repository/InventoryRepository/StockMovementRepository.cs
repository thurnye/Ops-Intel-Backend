using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class StockMovementRepository : BaseRepository<StockMovement>, IStockMovementRepository
{
    public StockMovementRepository(OperationIntelligenceDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<StockMovement>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(x => x.Warehouse)
            .Where(x => x.ProductId == productId)
            .OrderByDescending(x => x.MovementDateUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<StockMovement>> GetByWarehouseIdAsync(Guid warehouseId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(x => x.Product)
            .Where(x => x.WarehouseId == warehouseId)
            .OrderByDescending(x => x.MovementDateUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<StockMovement>> GetRecentAsync(int take = 50, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(x => x.Product)
            .Include(x => x.Warehouse)
            .OrderByDescending(x => x.MovementDateUtc)
            .Take(take)
            .ToListAsync(cancellationToken);
    }
}
