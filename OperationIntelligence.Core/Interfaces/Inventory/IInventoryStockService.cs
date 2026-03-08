namespace OperationIntelligence.Core;

public interface IInventoryStockService
{
    Task<InventoryStockResponse> StockInAsync(StockInRequest request, CancellationToken cancellationToken = default);
    Task<InventoryStockResponse> StockOutAsync(StockOutRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<InventoryStockResponse>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
}
