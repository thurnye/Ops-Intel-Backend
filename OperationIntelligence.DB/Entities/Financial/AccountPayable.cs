namespace OperationIntelligence.DB;

public class AccountPayable : AuditableEntity
{
    public Guid VendorBillId { get; set; }
    public VendorBill VendorBill { get; set; } = default!;

    public Guid VendorId { get; set; }

    public DateTime BillDate { get; set; }
    public DateTime DueDate { get; set; }

    public decimal OriginalAmount { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal OutstandingAmount { get; set; }

    public bool IsOverdue { get; set; } = false;
}
