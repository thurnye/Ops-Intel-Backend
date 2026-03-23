namespace OperationIntelligence.DB;

public class AccountReceivable : AuditableEntity
{
    public Guid InvoiceId { get; set; }
    public Invoice Invoice { get; set; } = default!;

    public Guid CustomerId { get; set; }

    public DateTime InvoiceDate { get; set; }
    public DateTime DueDate { get; set; }

    public decimal OriginalAmount { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal OutstandingAmount { get; set; }

    public bool IsOverdue { get; set; } = false;
}
