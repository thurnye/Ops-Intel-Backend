using OperationIntelligence.DB;

namespace OperationIntelligence.Core.Models.Production.Responses;

public class ProductionDowntimeResponse
{
    public Guid Id { get; set; }
    public Guid ProductionExecutionId { get; set; }
    public DowntimeReasonType Reason { get; set; }
    public string? ReasonDescription { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public decimal DurationMinutes { get; set; }
    public bool IsPlanned { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public string? UpdatedBy { get; set; }
}
