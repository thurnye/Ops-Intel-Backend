namespace OperationIntelligence.DB;

public class ProductionOutput : AuditableEntity
{
    public Guid ProductionOrderId { get; set; }
    public ProductionOrder ProductionOrder { get; set; } = default!;

    public Guid ProductId { get; set; }
    public Product Product { get; set; } = default!;

    public Guid WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = default!;

    public decimal QuantityProduced { get; set; }

    public Guid UnitOfMeasureId { get; set; }
    public UnitOfMeasure UnitOfMeasure { get; set; } = default!;

    public string? BatchNumber { get; set; }
    public string? LotNumber { get; set; }

    public DateTime OutputDate { get; set; }

    public Guid? StockMovementId { get; set; }
    public StockMovement? StockMovement { get; set; }

    public bool IsFinalOutput { get; set; } = true;

    public string? Notes { get; set; }
}