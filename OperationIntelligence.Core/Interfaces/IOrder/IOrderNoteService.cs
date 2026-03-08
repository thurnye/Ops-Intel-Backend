namespace OperationIntelligence.Core;

public interface IOrderNoteService
{
    Task<OrderNoteResponse> AddAsync(CreateOrderNoteRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OrderNoteResponse>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
}