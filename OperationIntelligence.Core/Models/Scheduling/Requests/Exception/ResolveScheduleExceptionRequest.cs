namespace OperationIntelligence.Core.Models.Scheduling.Requests.Exception;

public class ResolveScheduleExceptionRequest
{
    public DateTime ResolvedAtUtc { get; set; }
    public string? ResolutionNotes { get; set; }
}
