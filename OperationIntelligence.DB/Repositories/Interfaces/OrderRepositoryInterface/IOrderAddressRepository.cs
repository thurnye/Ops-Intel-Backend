namespace OperationIntelligence.DB;

public interface IOrderAddressRepository : IOrderBaseRepository<OrderAddress>
{
    Task<IReadOnlyList<OrderAddress>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<OrderAddress?> GetBillingAddressAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<OrderAddress?> GetShippingAddressAsync(Guid orderId, CancellationToken cancellationToken = default);
}