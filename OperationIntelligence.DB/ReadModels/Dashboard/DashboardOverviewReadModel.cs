namespace OperationIntelligence.DB;

public class DashboardOverviewReadModel
{
    public DashboardKpisReadModel Kpis { get; set; } = new();
    public List<DashboardAlertReadModel> Alerts { get; set; } = new();
    public List<DashboardModuleHealthReadModel> ModuleHealth { get; set; } = new();

    public BusinessPerformanceReadModel BusinessPerformance { get; set; } = new();
    public FinanceAnalyticsReadModel Finance { get; set; } = new();
    public InventoryAnalyticsReadModel Inventory { get; set; } = new();
    public ProductionAnalyticsReadModel Production { get; set; } = new();
    public ShipmentAnalyticsReadModel Shipment { get; set; } = new();

    public List<WorkflowPipelineReadModel> WorkflowPipeline { get; set; } = new();
    public List<ActivityFeedReadModel> ActivityFeed { get; set; } = new();

    public WorkforceSummaryReadModel WorkforceSummary { get; set; } = new();
    public ProcurementSummaryReadModel ProcurementSummary { get; set; } = new();
    public WarehouseSummaryReadModel WarehouseSummary { get; set; } = new();

    public List<RecentOrderReadModel> RecentOrders { get; set; } = new();
    public List<LowStockItemReadModel> LowStockItems { get; set; } = new();
}