namespace OperationIntelligence.DB;

public interface IOrderNoteRepository : IOrderBaseRepository<OrderNote>
{
    Task<IReadOnlyList<OrderNote>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OrderNote>> GetInternalNotesByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
}