using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class ShipmentItemResponse
{
    public Guid Id { get; set; }
    public Guid? OrderItemId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public Guid WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public Guid UnitOfMeasureId { get; set; }
    public string UnitOfMeasureName { get; set; } = string.Empty;
    public Guid? InventoryStockId { get; set; }
    public Guid? ProductionOrderId { get; set; }
    public string LineNumber { get; set; } = string.Empty;
    public decimal OrderedQuantity { get; set; }
    public decimal AllocatedQuantity { get; set; }
    public decimal PickedQuantity { get; set; }
    public decimal PackedQuantity { get; set; }
    public decimal ShippedQuantity { get; set; }
    public decimal DeliveredQuantity { get; set; }
    public decimal ReturnedQuantity { get; set; }
    public decimal UnitWeight { get; set; }
    public decimal UnitVolume { get; set; }
    public string? LotNumber { get; set; }
    public string? SerialNumber { get; set; }
    public DateTime? ExpiryDateUtc { get; set; }
    public ShipmentItemStatus Status { get; set; }
    public string? Notes { get; set; }
}
