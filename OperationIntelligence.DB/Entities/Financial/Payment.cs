namespace OperationIntelligence.DB;

public class Payment : AuditableEntity
{
    public string PaymentReference { get; set; } = default!;
    public DateTime PaymentDate { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    public Guid CustomerId { get; set; }

    public decimal AmountReceived { get; set; }
    public string CurrencyCode { get; set; } = "CAD";
    public string PaymentMethod { get; set; } = default!;
    public string? ExternalTransactionReference { get; set; }
    public string? Notes { get; set; }

    public ICollection<PaymentAllocation> Allocations { get; set; } = new List<PaymentAllocation>();
}
