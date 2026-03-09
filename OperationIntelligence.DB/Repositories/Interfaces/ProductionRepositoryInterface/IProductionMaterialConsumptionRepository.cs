namespace OperationIntelligence.DB;

public interface IProductionMaterialConsumptionRepository : IBaseRepository<ProductionMaterialConsumption>
{
    Task<IReadOnlyList<ProductionMaterialConsumption>> GetByProductionMaterialIssueIdAsync(Guid productionMaterialIssueId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProductionMaterialConsumption>> GetByProductionExecutionIdAsync(Guid productionExecutionId, CancellationToken cancellationToken = default);
}
