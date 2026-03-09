namespace OperationIntelligence.Core.Models.Scheduling.Responses.ScheduleOperation;

public class ScheduleOperationResponse
{
    public Guid Id { get; set; }

    public Guid ScheduleJobId { get; set; }
    public string JobNumber { get; set; } = string.Empty;

    public Guid RoutingStepId { get; set; }
    public string RoutingStepName { get; set; } = string.Empty;

    public Guid WorkCenterId { get; set; }
    public string WorkCenterName { get; set; } = string.Empty;

    public Guid? MachineId { get; set; }
    public string? MachineName { get; set; }

    public Guid? ProductionExecutionId { get; set; }

    public Guid? PlannedShiftId { get; set; }
    public string? PlannedShiftName { get; set; }

    public Guid? ActualShiftId { get; set; }
    public string? ActualShiftName { get; set; }

    public int SequenceNo { get; set; }
    public string OperationCode { get; set; } = string.Empty;
    public string OperationName { get; set; } = string.Empty;

    public DateTime PlannedStartUtc { get; set; }
    public DateTime PlannedEndUtc { get; set; }

    public DateTime? ActualStartUtc { get; set; }
    public DateTime? ActualEndUtc { get; set; }

    public decimal SetupTimeMinutes { get; set; }
    public decimal RunTimeMinutes { get; set; }
    public decimal QueueTimeMinutes { get; set; }
    public decimal WaitTimeMinutes { get; set; }
    public decimal MoveTimeMinutes { get; set; }

    public decimal PlannedQuantity { get; set; }
    public decimal CompletedQuantity { get; set; }
    public decimal ScrappedQuantity { get; set; }

    public int Status { get; set; }
    public string StatusName { get; set; } = string.Empty;

    public int DispatchStatus { get; set; }
    public string DispatchStatusName { get; set; } = string.Empty;

    public bool IsCriticalPath { get; set; }
    public bool IsBottleneckOperation { get; set; }
    public bool IsOutsourced { get; set; }

    public int PriorityScore { get; set; }
    public string? ConstraintReason { get; set; }
    public string? Notes { get; set; }

    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
