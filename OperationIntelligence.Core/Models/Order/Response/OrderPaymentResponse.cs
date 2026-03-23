using OrderPaymentStatus = global::PaymentStatus;

namespace OperationIntelligence.Core;

public class OrderPaymentResponse
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public string PaymentReference { get; set; } = default!;
    public PaymentMethod PaymentMethod { get; set; }
    public PaymentProvider PaymentProvider { get; set; }
    public PaymentTransactionType TransactionType { get; set; }
    public OrderPaymentStatus Status { get; set; }
    public decimal Amount { get; set; }
    public decimal FeeAmount { get; set; }
    public decimal NetAmount { get; set; }
    public decimal RefundedAmount { get; set; }
    public string CurrencyCode { get; set; } = default!;
    public DateTime PaymentDateUtc { get; set; }
}
