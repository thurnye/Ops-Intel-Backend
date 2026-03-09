namespace OperationIntelligence.Core.Models.Scheduling.Requests.SchedulePlan;

public class CreateSchedulePlanRequest
{
    public string PlanNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public Guid WarehouseId { get; set; }

    public DateTime PlanningStartDateUtc { get; set; }
    public DateTime PlanningEndDateUtc { get; set; }

    public int GenerationMode { get; set; }
    public int SchedulingStrategy { get; set; }

    public bool AutoSequenceEnabled { get; set; }
    public bool AutoDispatchEnabled { get; set; }

    public string TimeZone { get; set; } = "UTC";
}
