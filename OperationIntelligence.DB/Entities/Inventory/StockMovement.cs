namespace OperationIntelligence.DB;

public class StockMovement : AuditableEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = default!;

    public Guid WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = default!;

    public StockMovementType MovementType { get; set; }

    public decimal Quantity { get; set; }
    public decimal QuantityBefore { get; set; }
    public decimal QuantityAfter { get; set; }

    public string? ReferenceNumber { get; set; } // PO, SO, Adjustment Ref, Transfer Ref
    public string? Reason { get; set; }
    public string? Notes { get; set; }

    public Guid? RelatedWarehouseId { get; set; } // for transfer
    public Warehouse? RelatedWarehouse { get; set; }

    public DateTime MovementDateUtc { get; set; } = DateTime.UtcNow;
}
