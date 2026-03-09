namespace OperationIntelligence.Core.Models.Scheduling.Requests.ScheduleOperation;

public class PauseScheduleOperationRequest
{
    public string Reason { get; set; } = string.Empty;
    public string? Notes { get; set; }
}
