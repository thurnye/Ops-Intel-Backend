namespace OperationIntelligence.DB;

public class ReturnShipmentItem : AuditableEntity
{
    public Guid ReturnShipmentId { get; set; }
    public ReturnShipment ReturnShipment { get; set; } = default!;

    public Guid ShipmentItemId { get; set; }
    public ShipmentItem ShipmentItem { get; set; } = default!;

    public decimal ReturnedQuantity { get; set; }
    public string? ReturnCondition { get; set; }
    public string? InspectionResult { get; set; }
    public string? Notes { get; set; }
}