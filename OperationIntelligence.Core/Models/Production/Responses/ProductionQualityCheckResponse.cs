using OperationIntelligence.DB;

namespace OperationIntelligence.Core.Models.Production.Responses;

public class ProductionQualityCheckResponse
{
    public Guid Id { get; set; }
    public Guid ProductionOrderId { get; set; }
    public Guid? ProductionExecutionId { get; set; }
    public QualityCheckType CheckType { get; set; }
    public QualityCheckStatus Status { get; set; }
    public DateTime CheckDate { get; set; }
    public Guid CheckedByUserId { get; set; }
    public string? CheckedByUserName { get; set; }
    public string? CheckedByUserEmail { get; set; }
    public string? ReferenceStandard { get; set; }
    public string? Findings { get; set; }
    public string? CorrectiveAction { get; set; }
    public bool RequiresRework { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public string? UpdatedBy { get; set; }
}
