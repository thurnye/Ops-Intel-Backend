using OperationIntelligence.DB;

namespace OperationIntelligence.Core.Models.Production.Requests;

public class CreateProductionQualityCheckRequest
{
    public Guid ProductionOrderId { get; set; }
    public Guid? ProductionExecutionId { get; set; }
    public QualityCheckType CheckType { get; set; }
    public QualityCheckStatus Status { get; set; } = QualityCheckStatus.Pending;
    public DateTime CheckDate { get; set; }
    public string CheckedByUserId { get; set; } = string.Empty;
    public string? ReferenceStandard { get; set; }
    public string? Findings { get; set; }
    public string? CorrectiveAction { get; set; }
    public bool RequiresRework { get; set; }
    public string? Notes { get; set; }
}
