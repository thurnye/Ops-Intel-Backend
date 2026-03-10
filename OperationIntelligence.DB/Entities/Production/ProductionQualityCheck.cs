
namespace OperationIntelligence.DB;

public class ProductionQualityCheck : AuditableEntity
{
    public Guid ProductionOrderId { get; set; }
    public ProductionOrder ProductionOrder { get; set; } = default!;

    public Guid? ProductionExecutionId { get; set; }
    public ProductionExecution? ProductionExecution { get; set; }

    public QualityCheckType CheckType { get; set; }
    public QualityCheckStatus Status { get; set; } = QualityCheckStatus.Pending;

    public DateTime CheckDate { get; set; }

    public Guid CheckedByUserId { get; set; }
    public PlatformUser CheckedByUser { get; set; } = default!;

    public string? ReferenceStandard { get; set; }
    public string? Findings { get; set; }
    public string? CorrectiveAction { get; set; }

    public bool RequiresRework { get; set; }

    public string? Notes { get; set; }
}
