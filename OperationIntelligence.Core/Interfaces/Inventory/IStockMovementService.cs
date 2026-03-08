namespace OperationIntelligence.Core;

public interface IStockMovementService
{
    Task<IReadOnlyList<StockMovementResponse>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StockMovementResponse>> GetByWarehouseIdAsync(Guid warehouseId, CancellationToken cancellationToken = default);
}
