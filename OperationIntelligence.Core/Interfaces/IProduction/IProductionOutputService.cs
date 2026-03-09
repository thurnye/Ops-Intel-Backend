namespace OperationIntelligence.Core;

public interface IProductionOutputService
{
    Task<IReadOnlyList<ProductionOutputResponse>> GetByProductionOrderIdAsync(Guid productionOrderId, CancellationToken cancellationToken = default);
    Task<ProductionOutputResponse> CreateAsync(CreateProductionOutputRequest request, string? createdBy = null, CancellationToken cancellationToken = default);
}
