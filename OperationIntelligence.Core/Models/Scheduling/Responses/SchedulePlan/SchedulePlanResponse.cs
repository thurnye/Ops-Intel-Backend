namespace OperationIntelligence.Core.Models.Scheduling.Responses.SchedulePlan;

public class SchedulePlanResponse
{
    public Guid Id { get; set; }
    public string PlanNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public Guid WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;

    public DateTime PlanningStartDateUtc { get; set; }
    public DateTime PlanningEndDateUtc { get; set; }

    public int Status { get; set; }
    public string StatusName { get; set; } = string.Empty;

    public int GenerationMode { get; set; }
    public string GenerationModeName { get; set; } = string.Empty;

    public int SchedulingStrategy { get; set; }
    public string SchedulingStrategyName { get; set; } = string.Empty;

    public bool AutoSequenceEnabled { get; set; }
    public bool AutoDispatchEnabled { get; set; }

    public int VersionNumber { get; set; }
    public Guid? ParentPlanId { get; set; }

    public string TimeZone { get; set; } = string.Empty;
    public DateTime? ApprovedAtUtc { get; set; }
    public string? ApprovedBy { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
