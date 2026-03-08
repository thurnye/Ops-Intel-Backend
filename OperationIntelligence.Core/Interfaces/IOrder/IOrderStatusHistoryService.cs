namespace OperationIntelligence.Core;

public interface IOrderStatusHistoryService
{
    Task<OrderStatusHistoryResponse> AddAsync(CreateOrderStatusHistoryRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OrderStatusHistoryResponse>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<OrderStatusHistoryResponse?> GetLatestByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
}