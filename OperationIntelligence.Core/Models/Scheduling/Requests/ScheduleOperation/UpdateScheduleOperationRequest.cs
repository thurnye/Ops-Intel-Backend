namespace OperationIntelligence.Core.Models.Scheduling.Requests.ScheduleOperation;

public class UpdateScheduleOperationRequest
{
    public Guid WorkCenterId { get; set; }
    public Guid? MachineId { get; set; }
    public Guid? PlannedShiftId { get; set; }

    public DateTime PlannedStartUtc { get; set; }
    public DateTime PlannedEndUtc { get; set; }

    public decimal SetupTimeMinutes { get; set; }
    public decimal RunTimeMinutes { get; set; }
    public decimal QueueTimeMinutes { get; set; }
    public decimal WaitTimeMinutes { get; set; }
    public decimal MoveTimeMinutes { get; set; }

    public decimal PlannedQuantity { get; set; }

    public bool IsCriticalPath { get; set; }
    public bool IsBottleneckOperation { get; set; }
    public bool IsOutsourced { get; set; }

    public int PriorityScore { get; set; }
    public string? ConstraintReason { get; set; }
    public string? Notes { get; set; }
}
