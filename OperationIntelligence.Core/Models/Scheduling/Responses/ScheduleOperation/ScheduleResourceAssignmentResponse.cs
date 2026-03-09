namespace OperationIntelligence.Core.Models.Scheduling.Responses.ScheduleOperation;

public class ScheduleResourceAssignmentResponse
{
    public Guid Id { get; set; }
    public Guid ScheduleOperationId { get; set; }
    public Guid ResourceId { get; set; }
    public int ResourceType { get; set; }
    public string ResourceTypeName { get; set; } = string.Empty;
    public Guid? ShiftId { get; set; }
    public string? ShiftName { get; set; }
    public string AssignmentRole { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
    public DateTime AssignedStartUtc { get; set; }
    public DateTime AssignedEndUtc { get; set; }
    public decimal PlannedHours { get; set; }
    public decimal ActualHours { get; set; }
    public int Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public string? Notes { get; set; }
}
