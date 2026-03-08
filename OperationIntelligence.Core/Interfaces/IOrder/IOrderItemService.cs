namespace OperationIntelligence.Core;

public interface IOrderItemService
{
    Task<OrderItemResponse> AddAsync(CreateOrderItemRequest request, CancellationToken cancellationToken = default);
    Task<OrderItemResponse> UpdateAsync(Guid id, UpdateOrderItemRequest request, CancellationToken cancellationToken = default);
    Task RemoveAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OrderItemResponse>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
}