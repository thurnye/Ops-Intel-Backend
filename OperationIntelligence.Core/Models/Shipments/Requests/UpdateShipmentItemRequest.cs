using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class UpdateShipmentItemRequest
{
    public Guid WarehouseId { get; set; }
    public Guid UnitOfMeasureId { get; set; }
    public Guid? InventoryStockId { get; set; }
    public Guid? ProductionOrderId { get; set; }

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
