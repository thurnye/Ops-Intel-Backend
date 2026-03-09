namespace OperationIntelligence.Core.Models.Scheduling.Requests.ScheduleOperation;

public class CreateScheduleResourceAssignmentRequest
{
    public Guid ScheduleOperationId { get; set; }
    public Guid ResourceId { get; set; }
    public int ResourceType { get; set; }
    public Guid? ShiftId { get; set; }

    public string AssignmentRole { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }

    public DateTime AssignedStartUtc { get; set; }
    public DateTime AssignedEndUtc { get; set; }

    public decimal PlannedHours { get; set; }
    public string? Notes { get; set; }
}
