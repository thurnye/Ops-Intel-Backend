namespace OperationIntelligence.Core;

public interface IOrderAddressService
{
    Task<OrderAddressResponse> AddAsync(CreateOrderAddressRequest request, CancellationToken cancellationToken = default);
    Task<OrderAddressResponse> UpdateAsync(Guid id, UpdateOrderAddressRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OrderAddressResponse>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
}
