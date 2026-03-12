namespace OperationIntelligence.Core;

public class ProductionOrderMetricsSummaryResponse
{
    public int TotalOrders { get; set; }
    public int InProgressOrders { get; set; }
    public int PausedOrders { get; set; }
    public int CompletedOrders { get; set; }
    public int PlannedOrDraftOrders { get; set; }
}

public class RoutingMetricsSummaryResponse
{
    public int TotalRoutings { get; set; }
    public int ActiveRoutings { get; set; }
    public int DefaultRoutings { get; set; }
    public int ProductCoverage { get; set; }
}

public class MachineMetricsSummaryResponse
{
    public int TotalMachines { get; set; }
    public int RunningMachines { get; set; }
    public int MaintenanceMachines { get; set; }
    public int DownMachines { get; set; }
    public int WorkCentersRepresented { get; set; }
}

public class ProductionExecutionMetricsSummaryResponse
{
    public int TotalExecutions { get; set; }
    public int RunningExecutions { get; set; }
    public int PausedExecutions { get; set; }
    public int CompletedExecutions { get; set; }
}
