namespace OperationIntelligence.DB;

public class InvoiceLine : AuditableEntity
{
    public Guid InvoiceId { get; set; }
    public Invoice Invoice { get; set; } = default!;

    public int LineNumber { get; set; }

    public Guid? OrderLineId { get; set; }
    public Guid? ProductId { get; set; }

    public string Description { get; set; } = default!;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal LineTotal { get; set; }
}
