namespace OperationIntelligence.Core.Models.Scheduling.Requests.SchedulePlan;

public class UpdateSchedulePlanRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public DateTime PlanningStartDateUtc { get; set; }
    public DateTime PlanningEndDateUtc { get; set; }

    public int GenerationMode { get; set; }
    public int SchedulingStrategy { get; set; }

    public bool AutoSequenceEnabled { get; set; }
    public bool AutoDispatchEnabled { get; set; }

    public string TimeZone { get; set; } = "UTC";
    public bool IsActive { get; set; } = true;
}
