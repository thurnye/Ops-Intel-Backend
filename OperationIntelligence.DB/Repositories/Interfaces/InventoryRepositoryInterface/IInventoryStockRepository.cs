namespace OperationIntelligence.DB;

public interface IInventoryStockRepository : IBaseRepository<InventoryStock>
{
    Task<InventoryStock?> GetByProductAndWarehouseAsync(Guid productId, Guid warehouseId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<InventoryStock>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<InventoryStock>> GetByWarehouseIdAsync(Guid warehouseId, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalAvailableStockAsync(Guid productId, CancellationToken cancellationToken = default);
}
