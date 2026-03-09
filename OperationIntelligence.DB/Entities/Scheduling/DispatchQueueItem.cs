namespace OperationIntelligence.DB;

public class DispatchQueueItem : AuditableEntity
{
    public Guid ScheduleOperationId { get; set; }
    public ScheduleOperation ScheduleOperation { get; set; } = default!;

    public Guid WorkCenterId { get; set; }
    public WorkCenter WorkCenter { get; set; } = default!;

    public Guid? MachineId { get; set; }
    public Machine? Machine { get; set; }

    public int QueuePosition { get; set; }
    public int PriorityScore { get; set; }

    public DispatchStatus Status { get; set; } = DispatchStatus.NotDispatched;

    public DateTime? ReleasedAtUtc { get; set; }
    public DateTime? AcknowledgedAtUtc { get; set; }

    public string? DispatchNotes { get; set; }

    public bool IsActive { get; set; } = true;
}
