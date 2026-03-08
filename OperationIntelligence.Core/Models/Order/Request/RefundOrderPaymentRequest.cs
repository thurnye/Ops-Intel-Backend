namespace OperationIntelligence.Core;

public class RefundOrderPaymentRequest
{
    public Guid OrderPaymentId { get; set; }
    public decimal RefundAmount { get; set; }
    public string? Reason { get; set; }
    public string RefundedBy { get; set; } = default!;
}