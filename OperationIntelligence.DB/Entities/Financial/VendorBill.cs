namespace OperationIntelligence.DB;

public class VendorBill : AuditableEntity
{
    public string BillNumber { get; set; } = default!;
    public Guid VendorId { get; set; }

    public DateTime BillDate { get; set; }
    public DateTime DueDate { get; set; }

    public VendorBillStatus Status { get; set; } = VendorBillStatus.Draft;

    public Guid? ShipmentId { get; set; }
    public Guid? ProductionBatchId { get; set; }
    public Guid? PurchaseReferenceId { get; set; }

    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal OutstandingAmount { get; set; }

    public string CurrencyCode { get; set; } = "CAD";
    public string? Notes { get; set; }

    public ICollection<VendorBillLine> Lines { get; set; } = new List<VendorBillLine>();
    public AccountPayable? AccountPayable { get; set; }
}
