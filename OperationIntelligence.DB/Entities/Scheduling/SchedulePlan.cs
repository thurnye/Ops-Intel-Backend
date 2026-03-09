namespace OperationIntelligence.DB;

public class SchedulePlan : AuditableEntity
{
    public string PlanNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public Guid WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = default!;

    public DateTime PlanningStartDateUtc { get; set; }
    public DateTime PlanningEndDateUtc { get; set; }

    public SchedulePlanStatus Status { get; set; } = SchedulePlanStatus.Draft;
    public ScheduleGenerationMode GenerationMode { get; set; } = ScheduleGenerationMode.Manual;
    public SchedulingStrategy SchedulingStrategy { get; set; } = SchedulingStrategy.FiniteCapacity;

    public bool AutoSequenceEnabled { get; set; }
    public bool AutoDispatchEnabled { get; set; }

    public int VersionNumber { get; set; } = 1;

    public Guid? ParentPlanId { get; set; }
    public SchedulePlan? ParentPlan { get; set; }

    public string TimeZone { get; set; } = "UTC";

    public DateTime? ApprovedAtUtc { get; set; }
    public string? ApprovedBy { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<SchedulePlan> ChildPlans { get; set; } = new List<SchedulePlan>();
    public ICollection<ScheduleJob> ScheduleJobs { get; set; } = new List<ScheduleJob>();
    public ICollection<ScheduleException> ScheduleExceptions { get; set; } = new List<ScheduleException>();
    public ICollection<ScheduleRevision> ScheduleRevisions { get; set; } = new List<ScheduleRevision>();
    public ICollection<ScheduleRescheduleHistory> RescheduleHistories { get; set; } = new List<ScheduleRescheduleHistory>();
    public ICollection<ScheduleStatusHistory> StatusHistories { get; set; } = new List<ScheduleStatusHistory>();
}
