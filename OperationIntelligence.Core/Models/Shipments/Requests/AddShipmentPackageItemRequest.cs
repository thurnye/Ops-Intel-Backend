namespace OperationIntelligence.Core;

public class AddShipmentPackageItemRequest
{
    public Guid ShipmentItemId { get; set; }
    public decimal Quantity { get; set; }
}
