using OrderPaymentStatus = global::PaymentStatus;

namespace OperationIntelligence.Core;

public class OrderResponse
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = default!;
    public OrderStatus Status { get; set; }
    public OrderPaymentStatus PaymentStatus { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal OutstandingAmount { get; set; }
    public string CurrencyCode { get; set; } = default!;
}
