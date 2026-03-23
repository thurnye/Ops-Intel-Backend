namespace OperationIntelligence.DB;

public class DashboardOverviewReadModel
{
    public DashboardKpisReadModel Kpis { get; set; } = new();
    public DashboardKpiComparisonReadModel KpiComparison { get; set; } = new();

    public List<DashboardAlertReadModel> Alerts { get; set; } = [];
    public List<DashboardModuleHealthReadModel> ModuleHealth { get; set; } = [];

    public BusinessPerformanceReadModel BusinessPerformance { get; set; } = new();
    public FinanceAnalyticsReadModel Finance { get; set; } = new();
    public InventoryAnalyticsReadModel Inventory { get; set; } = new();
    public ProductionAnalyticsReadModel Production { get; set; } = new();
    public ShipmentAnalyticsReadModel Shipment { get; set; } = new();

    public DashboardOperationsReadModel Operations { get; set; } = new();

    public List<WorkflowPipelineReadModel> WorkflowPipeline { get; set; } = [];
    public List<ActivityFeedReadModel> ActivityFeed { get; set; } = [];

    public WorkforceSummaryReadModel WorkforceSummary { get; set; } = new();
    public ProcurementSummaryReadModel ProcurementSummary { get; set; } = new();
    public WarehouseSummaryReadModel WarehouseSummary { get; set; } = new();

    public List<RecentOrderReadModel> RecentOrders { get; set; } = [];
    public List<LowStockItemReadModel> LowStockItems { get; set; } = [];
}

public class DashboardKpiComparisonReadModel
{
    public decimal RevenueChangePercent { get; set; }
    public decimal OrdersInProgressChangePercent { get; set; }
    public decimal ProductionEfficiencyChangePercent { get; set; }
    public decimal InventoryValueChangePercent { get; set; }
    public decimal ShipmentsPendingChangePercent { get; set; }
    public decimal CriticalAlertsChangePercent { get; set; }
}

public class DashboardOperationsReadModel
{
    public List<WeekdayMetricReadModel> WeeklyOrdersVsShipments { get; set; } = [];
    public List<DashboardProgressMetricReadModel> TeamTaskCompletion { get; set; } = [];
    public List<PieSliceReadModel> ShipmentInventoryMix { get; set; } = [];
    public DashboardInsightReadModel Insight { get; set; } = new();
     public int DelayedShipmentCount { get; set; }
}

public class WeekdayMetricReadModel
{
    public string Label { get; set; } = string.Empty;
    public decimal Orders { get; set; }
    public decimal Shipments { get; set; }
}

public class DashboardProgressMetricReadModel
{
    public string Label { get; set; } = string.Empty;
    public decimal Value { get; set; }
}

public class DashboardInsightReadModel
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

