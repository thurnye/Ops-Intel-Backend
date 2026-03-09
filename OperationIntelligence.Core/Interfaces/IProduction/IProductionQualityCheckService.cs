namespace OperationIntelligence.Core;

public interface IProductionQualityCheckService
{
    Task<IReadOnlyList<ProductionQualityCheckResponse>> GetByProductionOrderIdAsync(Guid productionOrderId, CancellationToken cancellationToken = default);
    Task<ProductionQualityCheckResponse> CreateAsync(CreateProductionQualityCheckRequest request, string? createdBy = null, CancellationToken cancellationToken = default);
}
