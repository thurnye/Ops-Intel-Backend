namespace OperationIntelligence.DB;

public class ProductionMaterialConsumption : AuditableEntity
{
    public Guid ProductionMaterialIssueId { get; set; }
    public ProductionMaterialIssue ProductionMaterialIssue { get; set; } = default!;

    public Guid? ProductionExecutionId { get; set; }
    public ProductionExecution? ProductionExecution { get; set; }

    public decimal ConsumedQuantity { get; set; }

    public DateTime ConsumptionDate { get; set; }

    public string? Notes { get; set; }
}