namespace OperationIntelligence.Core.Models.Scheduling.Shared;

public class SchedulingUserActionDto
{
    public string? PerformedBy { get; set; }
    public DateTime PerformedAtUtc { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }
}
