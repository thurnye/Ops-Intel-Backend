namespace OperationIntelligence.DB;

public interface IProductionQualityCheckRepository : IBaseRepository<ProductionQualityCheck>
{
    Task<IReadOnlyList<ProductionQualityCheck>> GetByProductionOrderIdAsync(Guid productionOrderId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProductionQualityCheck>> GetByProductionExecutionIdAsync(Guid productionExecutionId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProductionQualityCheck>> GetByStatusAsync(QualityCheckStatus status, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProductionQualityCheck>> GetByCheckTypeAsync(QualityCheckType checkType, CancellationToken cancellationToken = default);
}
