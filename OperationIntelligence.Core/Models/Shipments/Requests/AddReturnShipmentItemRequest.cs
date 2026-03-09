namespace OperationIntelligence.Core;

public class AddReturnShipmentItemRequest
{
    public Guid ShipmentItemId { get; set; }
    public decimal ReturnedQuantity { get; set; }
    public string? ReturnCondition { get; set; }
    public string? InspectionResult { get; set; }
    public string? Notes { get; set; }
}
