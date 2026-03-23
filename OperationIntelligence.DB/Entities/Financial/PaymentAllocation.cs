namespace OperationIntelligence.DB;

public class PaymentAllocation : AuditableEntity
{
    public Guid PaymentId { get; set; }
    public Payment Payment { get; set; } = default!;

    public Guid InvoiceId { get; set; }
    public Invoice Invoice { get; set; } = default!;

    public decimal AmountApplied { get; set; }
    public DateTime AllocationDate { get; set; }
}
