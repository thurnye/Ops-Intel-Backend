namespace OperationIntelligence.Core;

public interface IProductionScrapService
{
    Task<IReadOnlyList<ProductionScrapResponse>> GetByProductionOrderIdAsync(Guid productionOrderId, CancellationToken cancellationToken = default);
    Task<ProductionScrapResponse> CreateAsync(CreateProductionScrapRequest request, string? createdBy = null, CancellationToken cancellationToken = default);
}
