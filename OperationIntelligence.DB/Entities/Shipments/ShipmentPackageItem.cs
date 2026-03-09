namespace OperationIntelligence.DB;

public class ShipmentPackageItem : AuditableEntity
{
    public Guid ShipmentPackageId { get; set; }
    public ShipmentPackage ShipmentPackage { get; set; } = default!;

    public Guid ShipmentItemId { get; set; }
    public ShipmentItem ShipmentItem { get; set; } = default!;

    public decimal Quantity { get; set; }
}