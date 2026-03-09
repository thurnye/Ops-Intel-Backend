namespace OperationIntelligence.DB;

public interface IProductionMaterialIssueRepository : IBaseRepository<ProductionMaterialIssue>
{
    Task<IReadOnlyList<ProductionMaterialIssue>> GetByProductionOrderIdAsync(Guid productionOrderId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProductionMaterialIssue>> GetByMaterialProductIdAsync(Guid materialProductId, CancellationToken cancellationToken = default);

    Task<ProductionMaterialIssue?> GetWithConsumptionsAsync(Guid id, CancellationToken cancellationToken = default);
}
