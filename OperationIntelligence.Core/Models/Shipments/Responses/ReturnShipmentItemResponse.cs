namespace OperationIntelligence.Core;

public class ReturnShipmentItemResponse
{
    public Guid Id { get; set; }
    public Guid ShipmentItemId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal ReturnedQuantity { get; set; }
    public string? ReturnCondition { get; set; }
    public string? InspectionResult { get; set; }
    public string? Notes { get; set; }
}
