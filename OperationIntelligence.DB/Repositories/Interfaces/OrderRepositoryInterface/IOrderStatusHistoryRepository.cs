namespace OperationIntelligence.DB;

public interface IOrderStatusHistoryRepository : IOrderBaseRepository<OrderStatusHistory>
{
    Task<IReadOnlyList<OrderStatusHistory>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<OrderStatusHistory?> GetLatestByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
}