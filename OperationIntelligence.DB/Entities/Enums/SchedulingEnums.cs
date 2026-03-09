namespace OperationIntelligence.DB;

public enum SchedulePlanStatus
{
    Draft = 1,
    Published = 2,
    InProgress = 3,
    Closed = 4,
    Cancelled = 5
}

public enum ScheduleGenerationMode
{
    Manual = 1,
    SemiAutomatic = 2,
    Automatic = 3
}

public enum SchedulingStrategy
{
    Forward = 1,
    Backward = 2,
    FiniteCapacity = 3,
    InfiniteCapacity = 4,
    ConstraintBased = 5
}

public enum SchedulePriority
{
    Low = 1,
    Normal = 2,
    High = 3,
    Urgent = 4,
    Critical = 5
}

public enum ScheduleJobStatus
{
    Unscheduled = 1,
    Scheduled = 2,
    Released = 3,
    Running = 4,
    Paused = 5,
    Completed = 6,
    Delayed = 7,
    Blocked = 8,
    Cancelled = 9
}

public enum ScheduleOperationStatus
{
    Pending = 1,
    Ready = 2,
    Scheduled = 3,
    Released = 4,
    Running = 5,
    Paused = 6,
    Completed = 7,
    Delayed = 8,
    Blocked = 9,
    Cancelled = 10
}

public enum DispatchStatus
{
    NotDispatched = 1,
    Dispatched = 2,
    Acknowledged = 3,
    InExecution = 4,
    Completed = 5,
    Skipped = 6
}

public enum DependencyType
{
    FinishToStart = 1,
    StartToStart = 2,
    FinishToFinish = 3,
    StartToFinish = 4
}

public enum ResourceType
{
    WorkCenter = 1,
    Machine = 2,
    Employee = 3,
    Tool = 4,
    Vehicle = 5,
    ExternalVendor = 6
}

public enum AssignmentStatus
{
    Planned = 1,
    Confirmed = 2,
    Reassigned = 3,
    Removed = 4
}

public enum CalendarExceptionType
{
    Holiday = 1,
    Maintenance = 2,
    Breakdown = 3,
    Training = 4,
    Shutdown = 5,
    OvertimeWindow = 6,
    Blocked = 7
}

public enum CapacityReservationStatus
{
    Reserved = 1,
    Released = 2,
    Consumed = 3,
    Cancelled = 4
}

public enum MaterialReadinessStatus
{
    NotChecked = 1,
    Ready = 2,
    PartiallyReady = 3,
    Shortage = 4,
    Reserved = 5,
    WaitingTransfer = 6,
    Blocked = 7
}

public enum OperationConstraintType
{
    PredecessorOperation = 1,
    MaterialAvailability = 2,
    QualityRelease = 3,
    ToolAvailability = 4,
    ExternalProcessCompletion = 5,
    LaborAvailability = 6
}

public enum ScheduleExceptionType
{
    MaterialShortage = 1,
    MachineBreakdown = 2,
    LaborUnavailable = 3,
    QualityHold = 4,
    UpstreamDelay = 5,
    CapacityConflict = 6,
    DueDateRisk = 7,
    ManualOverride = 8
}

public enum ScheduleExceptionSeverity
{
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4
}

public enum ScheduleExceptionStatus
{
    Open = 1,
    Investigating = 2,
    Resolved = 3,
    Ignored = 4
}
