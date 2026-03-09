namespace OperationIntelligence.Core.Models.Scheduling.Responses.SchedulePlan;

public class ScheduleJobSummaryResponse
{
    public Guid Id { get; set; }
    public string JobNumber { get; set; } = string.Empty;
    public string JobName { get; set; } = string.Empty;
    public decimal PlannedQuantity { get; set; }
    public int Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public DateTime? DueDateUtc { get; set; }
}
