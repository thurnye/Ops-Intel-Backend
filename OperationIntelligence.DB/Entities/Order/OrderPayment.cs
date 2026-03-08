namespace OperationIntelligence.DB;

public class OrderPayment : OrderBaseEntity
{
    public Guid OrderId { get; set; }
    public Order Order { get; set; } = default!;

    public string PaymentReference { get; set; } = default!;
    public string? ExternalTransactionId { get; set; }
    public string? ExternalPaymentIntentId { get; set; }

    public PaymentMethod PaymentMethod { get; set; }
    public PaymentProvider PaymentProvider { get; set; } = PaymentProvider.Manual;
    public PaymentTransactionType TransactionType { get; set; } = PaymentTransactionType.Payment;
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    public decimal Amount { get; set; }
    public decimal FeeAmount { get; set; }
    public decimal NetAmount { get; set; }
    public string CurrencyCode { get; set; } = "CAD";

    public DateTime PaymentDateUtc { get; set; }
    public DateTime? ProcessedDateUtc { get; set; }

    public string? PayerName { get; set; }
    public string? PayerEmail { get; set; }
    public string? Last4 { get; set; }

    public string? AuthorizationCode { get; set; }
    public string? ReceiptNumber { get; set; }

    public string? FailureReason { get; set; }
    public string? Notes { get; set; }

    public bool IsRefunded { get; set; }
    public decimal RefundedAmount { get; set; }

    public string? RecordedBy { get; set; }
    public bool IsActive { get; set; } = true;
}