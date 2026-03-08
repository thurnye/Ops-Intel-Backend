using OperationIntelligence.DB;

namespace OperationIntelligence.Core.Services;

public class OrderNoteService : IOrderNoteService
{
    private readonly IOrderNoteRepository _orderNoteRepository;
    private readonly IOrderRepository _orderRepository;

    public OrderNoteService(
        IOrderNoteRepository orderNoteRepository,
        IOrderRepository orderRepository)
    {
        _orderNoteRepository = orderNoteRepository;
        _orderRepository = orderRepository;
    }

    public async Task<OrderNoteResponse> AddAsync(CreateOrderNoteRequest request, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order == null || !order.IsActive)
            throw new KeyNotFoundException(OrderErrorMessages.OrderNotFound);

        if (string.IsNullOrWhiteSpace(request.Note))
            throw new InvalidOperationException(OrderErrorMessages.NoteIsRequired);

        var entity = new OrderNote
        {
            Id = Guid.NewGuid(),
            OrderId = request.OrderId,
            Note = request.Note.Trim(),
            IsInternal = request.IsInternal,
            CreatedBy = request.CreatedBy,
            CreatedAtUtc = DateTime.UtcNow,
            // IsActive = true
        };

        await _orderNoteRepository.AddAsync(entity, cancellationToken);
        await _orderNoteRepository.SaveChangesAsync(cancellationToken);

        return new OrderNoteResponse
        {
            Id = entity.Id,
            OrderId = entity.OrderId,
            Note = entity.Note,
            IsInternal = entity.IsInternal,
            CreatedBy = entity.CreatedBy,
            CreatedAtUtc = entity.CreatedAtUtc
        };
    }

    public async Task<IReadOnlyList<OrderNoteResponse>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var rows = await _orderNoteRepository.GetByOrderIdAsync(orderId, cancellationToken);

        return rows.Select(note => new OrderNoteResponse
        {
            Id = note.Id,
            OrderId = note.OrderId,
            Note = note.Note,
            IsInternal = note.IsInternal,
            CreatedBy = note.CreatedBy,
            CreatedAtUtc = note.CreatedAtUtc
        }).ToList();
    }
}