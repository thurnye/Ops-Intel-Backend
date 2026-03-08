namespace OperationIntelligence.Core;

public class OrderStatusHistoryResponse
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public OrderStatus FromStatus { get; set; }
    public OrderStatus ToStatus { get; set; }
    public string? Reason { get; set; }
    public string? ChangedBy { get; set; }
    public DateTime ChangedAtUtc { get; set; }
    public string? Comments { get; set; }
}