namespace OperationIntelligence.DB;

public class ProductionOrder : AuditableEntity
{
    public string ProductionOrderNumber { get; set; } = string.Empty;

    public Guid ProductId { get; set; }
    public Product Product { get; set; } = default!;

    public decimal PlannedQuantity { get; set; }
    public decimal ProducedQuantity { get; set; }
    public decimal ScrapQuantity { get; set; }
    public decimal RemainingQuantity { get; set; }

    public Guid UnitOfMeasureId { get; set; }
    public UnitOfMeasure UnitOfMeasure { get; set; } = default!;

    public Guid? BillOfMaterialId { get; set; }
    public BillOfMaterial? BillOfMaterial { get; set; }

    public Guid? RoutingId { get; set; }
    public Routing? Routing { get; set; }

    public Guid WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = default!;

    public DateTime PlannedStartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }

    public DateTime? ActualStartDate { get; set; }
    public DateTime? ActualEndDate { get; set; }

    public ProductionOrderStatus Status { get; set; } = ProductionOrderStatus.Draft;
    public ProductionPriority Priority { get; set; } = ProductionPriority.Medium;

    public ProductionSourceType SourceType { get; set; } = ProductionSourceType.Manual;
    public Guid? SourceReferenceId { get; set; }

    public string? BatchNumber { get; set; }
    public string? LotNumber { get; set; }

    public string? Notes { get; set; }

    public decimal EstimatedMaterialCost { get; set; }
    public decimal EstimatedLaborCost { get; set; }
    public decimal EstimatedOverheadCost { get; set; }

    public decimal ActualMaterialCost { get; set; }
    public decimal ActualLaborCost { get; set; }
    public decimal ActualOverheadCost { get; set; }

    public bool IsReleased { get; set; }
    public bool IsClosed { get; set; }

    public ICollection<ProductionExecution> Executions { get; set; } = new List<ProductionExecution>();
    public ICollection<ProductionMaterialIssue> MaterialIssues { get; set; } = new List<ProductionMaterialIssue>();
    public ICollection<ProductionOutput> Outputs { get; set; } = new List<ProductionOutput>();
    public ICollection<ProductionScrap> Scraps { get; set; } = new List<ProductionScrap>();
    public ICollection<ProductionQualityCheck> QualityChecks { get; set; } = new List<ProductionQualityCheck>();
}