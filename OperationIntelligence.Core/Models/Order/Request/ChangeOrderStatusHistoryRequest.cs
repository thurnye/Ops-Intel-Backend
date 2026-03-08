namespace OperationIntelligence.Core;

public class CreateOrderStatusHistoryRequest
{
    public Guid OrderId { get; set; }
    public OrderStatus FromStatus { get; set; }
    public OrderStatus ToStatus { get; set; }
    public string? Reason { get; set; }
    public string ChangedBy { get; set; } = default!;
    public string? Comments { get; set; }
}