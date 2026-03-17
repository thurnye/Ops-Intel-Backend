namespace OperationIntelligence.Core;

public interface IProductionOrderService
{
    Task<ProductionOrderResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResponse<ProductionOrderSummaryResponse>> GetPagedAsync(ProductionOrderQueryRequest request, CancellationToken cancellationToken = default);
    Task<ProductionOrderMetricsSummaryResponse> GetSummaryAsync(ProductionOrderQueryRequest request, CancellationToken cancellationToken = default);
    Task<ProductionOrderResponse> CreateAsync(CreateProductionOrderRequest request, string? createdBy = null, CancellationToken cancellationToken = default);
    Task<BulkCreateResponse<ProductionOrderResponse>> CreateBulkAsync(BulkCreateRequest<CreateProductionOrderRequest> request, string? createdBy = null, CancellationToken cancellationToken = default);
    Task<ProductionOrderResponse?> UpdateAsync(Guid id, UpdateProductionOrderRequest request, string? updatedBy = null, CancellationToken cancellationToken = default);
    Task<bool> ReleaseAsync(Guid id, string? updatedBy = null, CancellationToken cancellationToken = default);
    Task<bool> StartAsync(Guid id, string? updatedBy = null, CancellationToken cancellationToken = default);
    Task<bool> CompleteAsync(Guid id, string? updatedBy = null, CancellationToken cancellationToken = default);
    Task<bool> CloseAsync(Guid id, string? updatedBy = null, CancellationToken cancellationToken = default);
    Task<bool> CancelAsync(Guid id, string? updatedBy = null, CancellationToken cancellationToken = default);
}
