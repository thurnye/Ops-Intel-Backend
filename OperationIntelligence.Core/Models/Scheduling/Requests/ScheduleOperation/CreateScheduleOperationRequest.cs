namespace OperationIntelligence.Core.Models.Scheduling.Requests.ScheduleOperation;

public class CreateScheduleOperationRequest
{
    public Guid ScheduleJobId { get; set; }

    public Guid RoutingStepId { get; set; }
    public Guid WorkCenterId { get; set; }
    public Guid? MachineId { get; set; }

    public Guid? PlannedShiftId { get; set; }

    public int SequenceNo { get; set; }
    public string OperationCode { get; set; } = string.Empty;
    public string OperationName { get; set; } = string.Empty;

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
