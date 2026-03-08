using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class OrderStatusHistoryService : IOrderStatusHistoryService
{
    private readonly IOrderStatusHistoryRepository _orderStatusHistoryRepository;
    private readonly IOrderRepository _orderRepository;

    public OrderStatusHistoryService(
        IOrderStatusHistoryRepository orderStatusHistoryRepository,
        IOrderRepository orderRepository)
    {
        _orderStatusHistoryRepository = orderStatusHistoryRepository;
        _orderRepository = orderRepository;
    }

    public async Task<OrderStatusHistoryResponse> AddAsync(CreateOrderStatusHistoryRequest request, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order == null || !order.IsActive)
            throw new KeyNotFoundException(OrderErrorMessages.OrderNotFound);

        var entity = new OrderStatusHistory
        {
            Id = Guid.NewGuid(),
            OrderId = request.OrderId,
            FromStatus = request.FromStatus,
            ToStatus = request.ToStatus,
            Reason = request.Reason,
            ChangedBy = request.ChangedBy,
            ChangedAtUtc = DateTime.UtcNow,
            Comments = request.Comments,
            // IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _orderStatusHistoryRepository.AddAsync(entity, cancellationToken);
        await _orderStatusHistoryRepository.SaveChangesAsync(cancellationToken);

        return new OrderStatusHistoryResponse
        {
            Id = entity.Id,
            OrderId = entity.OrderId,
            FromStatus = entity.FromStatus,
            ToStatus = entity.ToStatus,
            Reason = entity.Reason,
            ChangedBy = entity.ChangedBy,
            ChangedAtUtc = entity.ChangedAtUtc,
            Comments = entity.Comments
        };
    }

    public async Task<IReadOnlyList<OrderStatusHistoryResponse>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var rows = await _orderStatusHistoryRepository.GetByOrderIdAsync(orderId, cancellationToken);

        return rows.Select(x => new OrderStatusHistoryResponse
        {
            Id = x.Id,
            OrderId = x.OrderId,
            FromStatus = x.FromStatus,
            ToStatus = x.ToStatus,
            Reason = x.Reason,
            ChangedBy = x.ChangedBy,
            ChangedAtUtc = x.ChangedAtUtc,
            Comments = x.Comments
        }).ToList();
    }

    public async Task<OrderStatusHistoryResponse?> GetLatestByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var row = await _orderStatusHistoryRepository.GetLatestByOrderIdAsync(orderId, cancellationToken);
        if (row == null) return null;

        return new OrderStatusHistoryResponse
        {
            Id = row.Id,
            OrderId = row.OrderId,
            FromStatus = row.FromStatus,
            ToStatus = row.ToStatus,
            Reason = row.Reason,
            ChangedBy = row.ChangedBy,
            ChangedAtUtc = row.ChangedAtUtc,
            Comments = row.Comments
        };
    }
}