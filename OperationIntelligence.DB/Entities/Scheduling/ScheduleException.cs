namespace OperationIntelligence.DB;

public class ScheduleException : AuditableEntity
{
    public Guid? SchedulePlanId { get; set; }
    public SchedulePlan? SchedulePlan { get; set; }

    public Guid? ScheduleJobId { get; set; }
    public ScheduleJob? ScheduleJob { get; set; }

    public Guid? ScheduleOperationId { get; set; }
    public ScheduleOperation? ScheduleOperation { get; set; }

    public ScheduleExceptionType ExceptionType { get; set; }
    public ScheduleExceptionSeverity Severity { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public DateTime DetectedAtUtc { get; set; }
    public DateTime? ResolvedAtUtc { get; set; }

    public string? AssignedTo { get; set; }
    public ScheduleExceptionStatus Status { get; set; } = ScheduleExceptionStatus.Open;

    public string? ResolutionNotes { get; set; }
}
