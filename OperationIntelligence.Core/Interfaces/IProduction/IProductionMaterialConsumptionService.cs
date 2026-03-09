namespace OperationIntelligence.Core;

public interface IProductionMaterialConsumptionService
{
    Task<IReadOnlyList<ProductionMaterialConsumptionResponse>> GetByProductionExecutionIdAsync(Guid productionExecutionId, CancellationToken cancellationToken = default);
    Task<ProductionMaterialConsumptionResponse> CreateAsync(CreateProductionMaterialConsumptionRequest request, string? createdBy = null, CancellationToken cancellationToken = default);
}
