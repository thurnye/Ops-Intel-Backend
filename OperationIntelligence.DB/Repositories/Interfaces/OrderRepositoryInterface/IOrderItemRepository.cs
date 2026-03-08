namespace OperationIntelligence.DB;

public interface IOrderItemRepository : IOrderBaseRepository<OrderItem>
{
    Task<IReadOnlyList<OrderItem>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<OrderItem?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
}