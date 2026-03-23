namespace OperationIntelligence.DB;

public class VendorBillLine : AuditableEntity
{
    public Guid VendorBillId { get; set; }
    public VendorBill VendorBill { get; set; } = default!;

    public int LineNumber { get; set; }

    public Guid? ProductId { get; set; }
    public Guid? ShipmentItemId { get; set; }
    public Guid? ProductionMaterialId { get; set; }

    public string Description { get; set; } = default!;
    public decimal Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal LineTotal { get; set; }
}
