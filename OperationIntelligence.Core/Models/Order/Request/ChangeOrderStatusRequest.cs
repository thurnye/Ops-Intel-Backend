namespace OperationIntelligence.Core;

public class ChangeOrderStatusRequest
{
    public OrderStatus NewStatus { get; set; }
    public string? Reason { get; set; }
    public string? Comments { get; set; }
    public string ChangedBy { get; set; } = default!;
}
