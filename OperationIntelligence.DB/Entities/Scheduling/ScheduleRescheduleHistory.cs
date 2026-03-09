namespace OperationIntelligence.DB;

public class ScheduleRescheduleHistory : AuditableEntity
{
    public Guid? SchedulePlanId { get; set; }
    public SchedulePlan? SchedulePlan { get; set; }

    public Guid? ScheduleJobId { get; set; }
    public ScheduleJob? ScheduleJob { get; set; }

    public Guid? ScheduleOperationId { get; set; }
    public ScheduleOperation? ScheduleOperation { get; set; }

    public DateTime? OldPlannedStartUtc { get; set; }
    public DateTime? OldPlannedEndUtc { get; set; }
    public DateTime? NewPlannedStartUtc { get; set; }
    public DateTime? NewPlannedEndUtc { get; set; }

    public Guid? OldWorkCenterId { get; set; }
    public WorkCenter? OldWorkCenter { get; set; }

    public Guid? NewWorkCenterId { get; set; }
    public WorkCenter? NewWorkCenter { get; set; }

    public Guid? OldMachineId { get; set; }
    public Machine? OldMachine { get; set; }

    public Guid? NewMachineId { get; set; }
    public Machine? NewMachine { get; set; }

    public string ReasonCode { get; set; } = string.Empty;
    public string ReasonDescription { get; set; } = string.Empty;

    public DateTime ChangedAtUtc { get; set; }
}
