namespace OperationIntelligence.DB;

public interface IProductionOrderRepository : IBaseRepository<ProductionOrder>
{
    Task<ProductionOrder?> GetByProductionOrderNumberAsync(string productionOrderNumber, CancellationToken cancellationToken = default);

    Task<ProductionOrder?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProductionOrder>> GetByStatusAsync(ProductionOrderStatus status, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProductionOrder>> GetOpenOrdersAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProductionOrder>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProductionOrder>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

    Task<bool> ProductionOrderNumberExistsAsync(string productionOrderNumber, Guid? excludeId = null, CancellationToken cancellationToken = default);
}
