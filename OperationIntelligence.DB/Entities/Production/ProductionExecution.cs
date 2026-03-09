namespace OperationIntelligence.DB;

public class ProductionExecution : AuditableEntity
{
    public Guid ProductionOrderId { get; set; }
    public ProductionOrder ProductionOrder { get; set; } = default!;

    public Guid? RoutingStepId { get; set; }
    public RoutingStep? RoutingStep { get; set; }

    public Guid WorkCenterId { get; set; }
    public WorkCenter WorkCenter { get; set; } = default!;

    public Guid? MachineId { get; set; }
    public Machine? Machine { get; set; }

    public decimal PlannedQuantity { get; set; }
    public decimal CompletedQuantity { get; set; }
    public decimal ScrapQuantity { get; set; }

    public DateTime PlannedStartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }

    public DateTime? ActualStartDate { get; set; }
    public DateTime? ActualEndDate { get; set; }

    public decimal ActualSetupTimeMinutes { get; set; }
    public decimal ActualRunTimeMinutes { get; set; }
    public decimal ActualDowntimeMinutes { get; set; }

    public ExecutionStatus Status { get; set; } = ExecutionStatus.Pending;

    public string? Remarks { get; set; }

    public ICollection<ProductionMaterialConsumption> MaterialConsumptions { get; set; } = new List<ProductionMaterialConsumption>();
    public ICollection<ProductionLaborLog> LaborLogs { get; set; } = new List<ProductionLaborLog>();
    public ICollection<ProductionDowntime> Downtimes { get; set; } = new List<ProductionDowntime>();
    public ICollection<ProductionScrap> Scraps { get; set; } = new List<ProductionScrap>();
    public ICollection<ProductionQualityCheck> QualityChecks { get; set; } = new List<ProductionQualityCheck>();
}