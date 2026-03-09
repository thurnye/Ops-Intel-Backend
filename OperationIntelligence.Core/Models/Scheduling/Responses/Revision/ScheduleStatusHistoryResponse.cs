namespace OperationIntelligence.Core.Models.Scheduling.Responses.Revision;

public class ScheduleStatusHistoryResponse
{
    public Guid Id { get; set; }
    public Guid? SchedulePlanId { get; set; }
    public Guid? ScheduleJobId { get; set; }
    public Guid? ScheduleOperationId { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public string OldStatus { get; set; } = string.Empty;
    public string NewStatus { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public string? Notes { get; set; }
    public DateTime ChangedAtUtc { get; set; }
}
