namespace OperationIntelligence.Core.Models.Scheduling.Responses.SchedulePlan;

public class SchedulePlanDetailResponse : SchedulePlanResponse
{
    public int TotalJobs { get; set; }
    public int TotalOperations { get; set; }
    public int TotalExceptions { get; set; }
    public int TotalRevisions { get; set; }

    public List<ScheduleJobSummaryResponse> Jobs { get; set; } = new();
}
