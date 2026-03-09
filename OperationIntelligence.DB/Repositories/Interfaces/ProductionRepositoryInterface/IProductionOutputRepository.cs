namespace OperationIntelligence.DB;

public interface IProductionOutputRepository : IBaseRepository<ProductionOutput>
{
    Task<IReadOnlyList<ProductionOutput>> GetByProductionOrderIdAsync(Guid productionOrderId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProductionOutput>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProductionOutput>> GetFinalOutputsByProductionOrderIdAsync(Guid productionOrderId, CancellationToken cancellationToken = default);
}
