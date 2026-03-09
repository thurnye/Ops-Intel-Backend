namespace OperationIntelligence.DB;

public class ScheduleOperation : AuditableEntity
{
    public Guid ScheduleJobId { get; set; }
    public ScheduleJob ScheduleJob { get; set; } = default!;

    public Guid RoutingStepId { get; set; }
    public RoutingStep RoutingStep { get; set; } = default!;

    public Guid WorkCenterId { get; set; }
    public WorkCenter WorkCenter { get; set; } = default!;

    public Guid? MachineId { get; set; }
    public Machine? Machine { get; set; }

    public Guid? ProductionExecutionId { get; set; }
    public ProductionExecution? ProductionExecution { get; set; }

    public Guid? PlannedShiftId { get; set; }
    public Shift? PlannedShift { get; set; }

    public Guid? ActualShiftId { get; set; }
    public Shift? ActualShift { get; set; }

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

    public ScheduleOperationStatus Status { get; set; } = ScheduleOperationStatus.Pending;
    public DispatchStatus DispatchStatus { get; set; } = DispatchStatus.NotDispatched;

    public bool IsCriticalPath { get; set; }
    public bool IsBottleneckOperation { get; set; }
    public bool IsOutsourced { get; set; }

    public int PriorityScore { get; set; }
    public string? ConstraintReason { get; set; }
    public string? Notes { get; set; }

    public ICollection<ScheduleOperationDependency> PredecessorDependencies { get; set; } = new List<ScheduleOperationDependency>();
    public ICollection<ScheduleOperationDependency> SuccessorDependencies { get; set; } = new List<ScheduleOperationDependency>();
    public ICollection<ScheduleOperationConstraint> Constraints { get; set; } = new List<ScheduleOperationConstraint>();
    public ICollection<ScheduleOperationResourceOption> ResourceOptions { get; set; } = new List<ScheduleOperationResourceOption>();
    public ICollection<ScheduleResourceAssignment> ResourceAssignments { get; set; } = new List<ScheduleResourceAssignment>();
    public ICollection<CapacityReservation> CapacityReservations { get; set; } = new List<CapacityReservation>();
    public ICollection<DispatchQueueItem> DispatchQueueItems { get; set; } = new List<DispatchQueueItem>();
    public ICollection<ScheduleMaterialCheck> MaterialChecks { get; set; } = new List<ScheduleMaterialCheck>();
    public ICollection<ScheduleException> ScheduleExceptions { get; set; } = new List<ScheduleException>();
    public ICollection<ScheduleRescheduleHistory> RescheduleHistories { get; set; } = new List<ScheduleRescheduleHistory>();
    public ICollection<ScheduleStatusHistory> StatusHistories { get; set; } = new List<ScheduleStatusHistory>();
}
