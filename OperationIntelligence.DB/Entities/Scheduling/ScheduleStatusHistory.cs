namespace OperationIntelligence.DB;

public class ScheduleStatusHistory : AuditableEntity
{
    public Guid? SchedulePlanId { get; set; }
    public SchedulePlan? SchedulePlan { get; set; }

    public Guid? ScheduleJobId { get; set; }
    public ScheduleJob? ScheduleJob { get; set; }

    public Guid? ScheduleOperationId { get; set; }
    public ScheduleOperation? ScheduleOperation { get; set; }

    public string EntityType { get; set; } = string.Empty;
    public string OldStatus { get; set; } = string.Empty;
    public string NewStatus { get; set; } = string.Empty;

    public string? Reason { get; set; }
    public string? Notes { get; set; }

    public DateTime ChangedAtUtc { get; set; }
}
