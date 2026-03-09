namespace OperationIntelligence.Core.Models.Scheduling.Requests.ScheduleJob;

public class RescheduleJobRequest
{
    public DateTime? NewEarliestStartUtc { get; set; }
    public DateTime? NewLatestFinishUtc { get; set; }
    public DateTime? NewDueDateUtc { get; set; }

    public int? NewPriority { get; set; }

    public string ReasonCode { get; set; } = string.Empty;
    public string ReasonDescription { get; set; } = string.Empty;
}
