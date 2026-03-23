using OrderPaymentStatus = global::PaymentStatus;

namespace OperationIntelligence.Core;

public class OrderListItemResponse
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = default!;
    public string? CustomerName { get; set; }
    public OrderType OrderType { get; set; }
    public OrderStatus Status { get; set; }
    public OrderPaymentStatus PaymentStatus { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime OrderDateUtc { get; set; }
}
