namespace OperationIntelligence.Core;

public class ShipmentPackageItemResponse
{
    public Guid Id { get; set; }
    public Guid ShipmentItemId { get; set; }
    public string LineNumber { get; set; } = string.Empty;
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
}
