namespace OperationIntelligence.DB;

public class ShipmentItem : AuditableEntity
{
    public Guid ShipmentId { get; set; }
    public Shipment Shipment { get; set; } = default!;

    public Guid? OrderItemId { get; set; }
    public OrderItem? OrderItem { get; set; }

    public Guid ProductId { get; set; }
    public Product Product { get; set; } = default!;

    public Guid WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = default!;

    public Guid UnitOfMeasureId { get; set; }
    public UnitOfMeasure UnitOfMeasure { get; set; } = default!;

    public Guid? InventoryStockId { get; set; }
    public InventoryStock? InventoryStock { get; set; }

    public Guid? ProductionOrderId { get; set; }
    public ProductionOrder? ProductionOrder { get; set; }

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

    public ShipmentItemStatus Status { get; set; } = ShipmentItemStatus.Pending;
    public string? Notes { get; set; }

    public ICollection<ShipmentPackageItem> PackageItems { get; set; } = new List<ShipmentPackageItem>();
    public ICollection<ReturnShipmentItem> ReturnItems { get; set; } = new List<ReturnShipmentItem>();
}