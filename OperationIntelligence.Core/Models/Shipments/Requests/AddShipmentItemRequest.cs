namespace OperationIntelligence.Core;

public class AddShipmentItemRequest
{
    public Guid? OrderItemId { get; set; }
    public Guid ProductId { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid UnitOfMeasureId { get; set; }
    public Guid? InventoryStockId { get; set; }
    public Guid? ProductionOrderId { get; set; }

    public string LineNumber { get; set; } = string.Empty;

    public decimal OrderedQuantity { get; set; }
    public decimal AllocatedQuantity { get; set; }
    public decimal PickedQuantity { get; set; }
    public decimal PackedQuantity { get; set; }
    public decimal ShippedQuantity { get; set; }

    public decimal UnitWeight { get; set; }
    public decimal UnitVolume { get; set; }

    public string? LotNumber { get; set; }
    public string? SerialNumber { get; set; }
    public DateTime? ExpiryDateUtc { get; set; }

    public string? Notes { get; set; }
}
