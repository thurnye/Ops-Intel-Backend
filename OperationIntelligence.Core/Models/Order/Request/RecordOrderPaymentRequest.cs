namespace OperationIntelligence.Core;

public class RecordOrderPaymentRequest
{
    public Guid OrderId { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public PaymentProvider PaymentProvider { get; set; }
    public decimal Amount { get; set; }
    public decimal FeeAmount { get; set; }
    public string CurrencyCode { get; set; } = "CAD";

    public string? ExternalTransactionId { get; set; }
    public string? ExternalPaymentIntentId { get; set; }
    public string? PayerName { get; set; }
    public string? PayerEmail { get; set; }
    public string? Last4 { get; set; }
    public string? AuthorizationCode { get; set; }
    public string? ReceiptNumber { get; set; }
    public string? Notes { get; set; }
    public string RecordedBy { get; set; } = default!;
}