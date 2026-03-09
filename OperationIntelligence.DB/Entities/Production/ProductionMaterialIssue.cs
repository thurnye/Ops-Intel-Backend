namespace OperationIntelligence.DB;

public class ProductionMaterialIssue : AuditableEntity
{
    public Guid ProductionOrderId { get; set; }
    public ProductionOrder ProductionOrder { get; set; } = default!;

    public Guid MaterialProductId { get; set; }
    public Product MaterialProduct { get; set; } = default!;

    public Guid WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = default!;

    public decimal PlannedQuantity { get; set; }
    public decimal IssuedQuantity { get; set; }
    public decimal ReturnedQuantity { get; set; }

    public Guid UnitOfMeasureId { get; set; }
    public UnitOfMeasure UnitOfMeasure { get; set; } = default!;

    public string? BatchNumber { get; set; }
    public string? LotNumber { get; set; }

    public DateTime IssueDate { get; set; }

    public Guid? StockMovementId { get; set; }
    public StockMovement? StockMovement { get; set; }

    public string? Notes { get; set; }

    public ICollection<ProductionMaterialConsumption> Consumptions { get; set; } = new List<ProductionMaterialConsumption>();
}