namespace OperationIntelligence.Core;

public interface IProductionMaterialIssueService
{
    Task<IReadOnlyList<ProductionMaterialIssueResponse>> GetByProductionOrderIdAsync(Guid productionOrderId, CancellationToken cancellationToken = default);
    Task<ProductionMaterialIssueResponse> CreateAsync(CreateProductionMaterialIssueRequest request, string? createdBy = null, CancellationToken cancellationToken = default);
}
