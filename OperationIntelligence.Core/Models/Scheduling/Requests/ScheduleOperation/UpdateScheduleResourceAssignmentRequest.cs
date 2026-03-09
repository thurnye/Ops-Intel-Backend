namespace OperationIntelligence.Core.Models.Scheduling.Requests.ScheduleOperation;

public class UpdateScheduleResourceAssignmentRequest
{
    public Guid? ShiftId { get; set; }

    public string AssignmentRole { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }

    public DateTime AssignedStartUtc { get; set; }
    public DateTime AssignedEndUtc { get; set; }

    public decimal PlannedHours { get; set; }
    public decimal ActualHours { get; set; }

    public int Status { get; set; }
    public string? Notes { get; set; }
}
