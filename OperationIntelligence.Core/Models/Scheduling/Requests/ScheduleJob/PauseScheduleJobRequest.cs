namespace OperationIntelligence.Core.Models.Scheduling.Requests.ScheduleJob;

public class PauseScheduleJobRequest
{
    public string Reason { get; set; } = string.Empty;
    public string? Notes { get; set; }
}
