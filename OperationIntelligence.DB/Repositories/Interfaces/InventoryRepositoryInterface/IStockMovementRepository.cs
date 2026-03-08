

namespace OperationIntelligence.DB;

public interface IStockMovementRepository : IBaseRepository<StockMovement>
{
    Task<IReadOnlyList<StockMovement>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StockMovement>> GetByWarehouseIdAsync(Guid warehouseId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StockMovement>> GetRecentAsync(int take = 50, CancellationToken cancellationToken = default);
}
