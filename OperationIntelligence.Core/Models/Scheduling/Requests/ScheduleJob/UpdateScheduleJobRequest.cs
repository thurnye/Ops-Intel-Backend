namespace OperationIntelligence.Core.Models.Scheduling.Requests.ScheduleJob;

public class UpdateScheduleJobRequest
{
    public string JobName { get; set; } = string.Empty;
    public string? Notes { get; set; }

    public decimal PlannedQuantity { get; set; }

    public DateTime? EarliestStartUtc { get; set; }
    public DateTime? LatestFinishUtc { get; set; }
    public DateTime? DueDateUtc { get; set; }

    public int Priority { get; set; }
    public bool IsRushOrder { get; set; }
    public bool QualityHold { get; set; }
}
