namespace OperationIntelligence.Core;

public class DashboardOverviewResponse
{
    public DashboardKpisDto Kpis { get; set; } = new();
    public List<DashboardAlertDto> Alerts { get; set; } = new();
    public List<DashboardModuleHealthDto> ModuleHealth { get; set; } = new();

    public BusinessPerformanceDto BusinessPerformance { get; set; } = new();
    public FinanceAnalyticsDto Finance { get; set; } = new();
    public InventoryAnalyticsDto Inventory { get; set; } = new();
    public ProductionAnalyticsDto Production { get; set; } = new();
    public ShipmentAnalyticsDto Shipment { get; set; } = new();

    public List<WorkflowPipelineDto> WorkflowPipeline { get; set; } = new();
    public List<ActivityFeedDto> ActivityFeed { get; set; } = new();

    public WorkforceSummaryDto WorkforceSummary { get; set; } = new();
    public ProcurementSummaryDto ProcurementSummary { get; set; } = new();
    public WarehouseSummaryDto WarehouseSummary { get; set; } = new();

    public List<RecentOrderDto> RecentOrders { get; set; } = new();
    public List<LowStockItemDto> LowStockItems { get; set; } = new();
}
