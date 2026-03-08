using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class InventoryStockRepository : BaseRepository<InventoryStock>, IInventoryStockRepository
{
    public InventoryStockRepository(OperationIntelligenceDbContext context) : base(context)
    {
    }

    public async Task<InventoryStock?> GetByProductAndWarehouseAsync(Guid productId, Guid warehouseId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(x => x.ProductId == productId && x.WarehouseId == warehouseId, cancellationToken);
    }

    public async Task<IReadOnlyList<InventoryStock>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(x => x.Warehouse)
            .Where(x => x.ProductId == productId)
            .OrderBy(x => x.Warehouse.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<InventoryStock>> GetByWarehouseIdAsync(Guid warehouseId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(x => x.Product)
            .Where(x => x.WarehouseId == warehouseId)
            .OrderBy(x => x.Product.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<decimal> GetTotalAvailableStockAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(x => x.ProductId == productId)
            .SumAsync(x => x.QuantityAvailable, cancellationToken);
    }
}
