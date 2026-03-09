namespace OperationIntelligence.Core.Models.Scheduling.Requests.Exception;

public class CreateScheduleExceptionRequest
{
    public Guid? SchedulePlanId { get; set; }
    public Guid? ScheduleJobId { get; set; }
    public Guid? ScheduleOperationId { get; set; }

    public int ExceptionType { get; set; }
    public int Severity { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public DateTime DetectedAtUtc { get; set; }
    public string? AssignedTo { get; set; }
}
