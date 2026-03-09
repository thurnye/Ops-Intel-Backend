namespace OperationIntelligence.Core.Models.Scheduling.Responses.Exception;

public class ScheduleExceptionResponse
{
    public Guid Id { get; set; }
    public Guid? SchedulePlanId { get; set; }
    public Guid? ScheduleJobId { get; set; }
    public Guid? ScheduleOperationId { get; set; }
    public int ExceptionType { get; set; }
    public string ExceptionTypeName { get; set; } = string.Empty;
    public int Severity { get; set; }
    public string SeverityName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DetectedAtUtc { get; set; }
    public DateTime? ResolvedAtUtc { get; set; }
    public string? AssignedTo { get; set; }
    public int Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public string? ResolutionNotes { get; set; }
}
