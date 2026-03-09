namespace OperationIntelligence.DB;

public class ScheduleResourceAssignment : AuditableEntity
{
    public Guid ScheduleOperationId { get; set; }
    public ScheduleOperation ScheduleOperation { get; set; } = default!;

    public Guid ResourceId { get; set; }
    public ResourceType ResourceType { get; set; }

    public Guid? ShiftId { get; set; }
    public Shift? Shift { get; set; }

    public string AssignmentRole { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }

    public DateTime AssignedStartUtc { get; set; }
    public DateTime AssignedEndUtc { get; set; }

    public decimal PlannedHours { get; set; }
    public decimal ActualHours { get; set; }

    public AssignmentStatus Status { get; set; } = AssignmentStatus.Planned;
    public string? Notes { get; set; }
}
