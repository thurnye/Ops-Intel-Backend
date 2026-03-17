namespace OperationIntelligence.Core;

public interface IOrderService
{
    Task<OrderResponse> CreateAsync(CreateOrderRequest request, CancellationToken cancellationToken = default);
    Task<BulkCreateResponse<OrderResponse>> CreateBulkAsync(BulkCreateRequest<CreateOrderRequest> request, CancellationToken cancellationToken = default);
    Task<OrderResponse> UpdateAsync(Guid id, UpdateOrderRequest request, CancellationToken cancellationToken = default);
    Task<OrderDetailResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<OrderDetailResponse?> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default);
    Task<PagedResponse<OrderListItemResponse>> GetPagedAsync(OrderQueryRequest request, CancellationToken cancellationToken = default);
    Task<OrderOverviewMetricsSummaryResponse> GetOverviewMetricsSummaryAsync(CancellationToken cancellationToken = default);
    Task<OrderCustomerMetricsSummaryResponse> GetCustomerMetricsSummaryAsync(CancellationToken cancellationToken = default);
    Task<OrderResponse> ChangeStatusAsync(Guid id, ChangeOrderStatusRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
