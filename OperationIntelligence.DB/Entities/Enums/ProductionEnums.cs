namespace OperationIntelligence.DB;

public enum ProductionOrderStatus
{
    Draft = 1,
    Planned = 2,
    Released = 3,
    InProgress = 4,
    Paused = 5,
    Completed = 6,
    Closed = 7,
    Cancelled = 8
}

public enum ProductionPriority
{
    Low = 1,
    Medium = 2,
    High = 3,
    Urgent = 4
}

public enum ProductionSourceType
{
    Manual = 1,
    SalesOrder = 2,
    Replenishment = 3,
    Forecast = 4,
    TransferDemand = 5
}

public enum ExecutionStatus
{
    Pending = 1,
    Ready = 2,
    Running = 3,
    Paused = 4,
    Completed = 5,
    Cancelled = 6
}

public enum MachineStatus
{
    Idle = 1,
    Running = 2,
    Down = 3,
    Maintenance = 4,
    Retired = 5
}


public enum ScrapReasonType
{
    Defect = 1,
    Damage = 2,
    Overproduction = 3,
    SetupLoss = 4,
    Expired = 5,
    Contamination = 6,
    Other = 7
}

public enum DowntimeReasonType
{
    MachineBreakdown = 1,
    PowerFailure = 2,
    MaterialShortage = 3,
    Changeover = 4,
    Maintenance = 5,
    QualityIssue = 6,
    OperatorUnavailable = 7,
    Other = 8
}


public enum QualityCheckType
{
    Incoming = 1,
    InProcess = 2,
    Final = 3,
    ReworkVerification = 4
}

public enum QualityCheckStatus
{
    Pending = 1,
    Passed = 2,
    Failed = 3,
    ConditionalPass = 4
}


