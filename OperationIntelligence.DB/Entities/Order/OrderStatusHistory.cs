namespace OperationIntelligence.DB;


public class OrderStatusHistory : OrderBaseEntity
{
    public Guid OrderId { get; set; }
    public Order Order { get; set; } = default!;

    public OrderStatus FromStatus { get; set; }
    public OrderStatus ToStatus { get; set; }

    public string? Reason { get; set; }
    public string? ChangedBy { get; set; }
    public DateTime ChangedAtUtc { get; set; }

    public string? Comments { get; set; }
}