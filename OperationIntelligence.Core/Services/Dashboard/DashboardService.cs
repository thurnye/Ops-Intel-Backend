using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class DashboardService : IDashboardService
{
    private readonly IDashboardReadRepository _dashboardReadRepository;

    public DashboardService(IDashboardReadRepository dashboardReadRepository)
    {
        _dashboardReadRepository = dashboardReadRepository;
    }

    public async Task<DashboardOverviewResponse> GetOverviewAsync(
        DashboardFilterRequest request,
        CancellationToken cancellationToken = default)
    {
        var readModel = await _dashboardReadRepository.GetOverviewAsync(
            request.Range,
            request.Site,
            cancellationToken);

        return MapToResponse(readModel);
    }

    private static DashboardOverviewResponse MapToResponse(DashboardOverviewReadModel source)
    {
        return new DashboardOverviewResponse
        {
            Kpis = MapKpis(source.Kpis),
            Alerts = source.Alerts.Select(MapAlert).ToList(),
            ModuleHealth = source.ModuleHealth.Select(MapModuleHealth).ToList(),

            BusinessPerformance = MapBusinessPerformance(source.BusinessPerformance),
            Finance = MapFinance(source.Finance),
            Inventory = MapInventory(source.Inventory),
            Production = MapProduction(source.Production),
            Shipment = MapShipment(source.Shipment),

            WorkflowPipeline = source.WorkflowPipeline.Select(MapWorkflowPipeline).ToList(),
            ActivityFeed = source.ActivityFeed.Select(MapActivityFeed).ToList(),

            WorkforceSummary = MapWorkforceSummary(source.WorkforceSummary),
            ProcurementSummary = MapProcurementSummary(source.ProcurementSummary),
            WarehouseSummary = MapWarehouseSummary(source.WarehouseSummary),

            RecentOrders = source.RecentOrders.Select(MapRecentOrder).ToList(),
            LowStockItems = source.LowStockItems.Select(MapLowStockItem).ToList()
        };
    }

    private static DashboardKpisDto MapKpis(DashboardKpisReadModel source)
    {
        return new DashboardKpisDto
        {
            TotalRevenue = source.TotalRevenue,
            OrdersInProgress = source.OrdersInProgress,
            ProductionEfficiency = source.ProductionEfficiency,
            InventoryValue = source.InventoryValue,
            ShipmentsPending = source.ShipmentsPending,
            CriticalAlerts = source.CriticalAlerts
        };
    }

    private static DashboardAlertDto MapAlert(DashboardAlertReadModel source)
    {
        return new DashboardAlertDto
        {
            Title = source.Title,
            Detail = source.Detail,
            Severity = source.Severity
        };
    }

    private static DashboardModuleHealthDto MapModuleHealth(DashboardModuleHealthReadModel source)
    {
        return new DashboardModuleHealthDto
        {
            Module = source.Module,
            Status = source.Status,
            Value = source.Value,
            Note = source.Note
        };
    }

    private static BusinessPerformanceDto MapBusinessPerformance(BusinessPerformanceReadModel source)
    {
        return new BusinessPerformanceDto
        {
            MonthlyRevenueTrend = source.MonthlyRevenueTrend
                .Select(MapChartPoint)
                .ToList(),
            OnTimeShipmentRate = source.OnTimeShipmentRate,
            WarehouseCapacityUse = source.WarehouseCapacityUse,
            ApprovalQueue = source.ApprovalQueue
        };
    }

    private static FinanceAnalyticsDto MapFinance(FinanceAnalyticsReadModel source)
    {
        return new FinanceAnalyticsDto
        {
            RevenueExpenseTrend = source.RevenueExpenseTrend
                .Select(MapMultiSeriesPoint)
                .ToList(),
            ExpenseBreakdown = source.ExpenseBreakdown
                .Select(MapPieSlice)
                .ToList()
        };
    }

    private static InventoryAnalyticsDto MapInventory(InventoryAnalyticsReadModel source)
    {
        return new InventoryAnalyticsDto
        {
            TopLowStockItems = source.TopLowStockItems
                .Select(MapChartPoint)
                .ToList(),
            InventoryInflowOutflow = source.InventoryInflowOutflow
                .Select(MapMultiSeriesPoint)
                .ToList(),
            WarehouseStockComposition = source.WarehouseStockComposition
                .Select(MapWarehouseStockComposition)
                .ToList(),
            InventoryMixByCategory = source.InventoryMixByCategory
                .Select(MapPieSlice)
                .ToList()
        };
    }

    private static ProductionAnalyticsDto MapProduction(ProductionAnalyticsReadModel source)
    {
        return new ProductionAnalyticsDto
        {
            PlannedVsActualOutput = source.PlannedVsActualOutput
                .Select(MapMultiSeriesPoint)
                .ToList(),
            ProductionJobStatusMix = source.ProductionJobStatusMix
                .Select(MapPieSlice)
                .ToList(),
            ProductionLineEfficiency = source.ProductionLineEfficiency
                .Select(MapProductionEfficiencySeries)
                .ToList()
        };
    }

    private static ShipmentAnalyticsDto MapShipment(ShipmentAnalyticsReadModel source)
    {
        return new ShipmentAnalyticsDto
        {
            OnTimeVsDelayedTrend = source.OnTimeVsDelayedTrend
                .Select(MapMultiSeriesPoint)
                .ToList(),
            ShipmentStatusDistribution = source.ShipmentStatusDistribution
                .Select(MapPieSlice)
                .ToList()
        };
    }

    private static WorkflowPipelineDto MapWorkflowPipeline(WorkflowPipelineReadModel source)
    {
        return new WorkflowPipelineDto
        {
            Label = source.Label,
            Count = source.Count,
            Progress = source.Progress
        };
    }

    private static ActivityFeedDto MapActivityFeed(ActivityFeedReadModel source)
    {
        return new ActivityFeedDto
        {
            Text = source.Text,
            TimeUtc = source.TimeUtc,
            Type = source.Type
        };
    }

    private static WorkforceSummaryDto MapWorkforceSummary(WorkforceSummaryReadModel source)
    {
        return new WorkforceSummaryDto
        {
            ActiveStaff = source.ActiveStaff,
            ShiftCoverage = source.ShiftCoverage,
            OpenPositions = source.OpenPositions
        };
    }

    private static ProcurementSummaryDto MapProcurementSummary(ProcurementSummaryReadModel source)
    {
        return new ProcurementSummaryDto
        {
            OpenPurchaseOrders = source.OpenPurchaseOrders,
            AwaitingApproval = source.AwaitingApproval,
            SupplierSlaMet = source.SupplierSlaMet
        };
    }

    private static WarehouseSummaryDto MapWarehouseSummary(WarehouseSummaryReadModel source)
    {
        return new WarehouseSummaryDto
        {
            WarehousesActive = source.WarehousesActive,
            AveragePickAccuracy = source.AveragePickAccuracy,
            CrossDockUtilization = source.CrossDockUtilization
        };
    }

    private static RecentOrderDto MapRecentOrder(RecentOrderReadModel source)
    {
        return new RecentOrderDto
        {
            OrderNo = source.OrderNo,
            Customer = source.Customer,
            Module = source.Module,
            Amount = source.Amount,
            Status = source.Status,
            DueDate = source.DueDate,
            Warehouse = source.Warehouse
        };
    }

    private static LowStockItemDto MapLowStockItem(LowStockItemReadModel source)
    {
        return new LowStockItemDto
        {
            Sku = source.Sku,
            Item = source.Item,
            Warehouse = source.Warehouse,
            Stock = source.Stock,
            ReorderLevel = source.ReorderLevel,
            Status = source.Status
        };
    }

    private static ChartPointDto MapChartPoint(ChartPointReadModel source)
    {
        return new ChartPointDto
        {
            Label = source.Label,
            Value = source.Value
        };
    }

    private static MultiSeriesPointDto MapMultiSeriesPoint(MultiSeriesPointReadModel source)
    {
        return new MultiSeriesPointDto
        {
            Label = source.Label,
            Series1 = source.Series1,
            Series2 = source.Series2,
            Series3 = source.Series3
        };
    }

    private static PieSliceDto MapPieSlice(PieSliceReadModel source)
    {
        return new PieSliceDto
        {
            Label = source.Label,
            Value = source.Value
        };
    }

    private static WarehouseStockCompositionDto MapWarehouseStockComposition(WarehouseStockCompositionReadModel source)
    {
        return new WarehouseStockCompositionDto
        {
            Warehouse = source.Warehouse,
            RawMaterials = source.RawMaterials,
            FinishedGoods = source.FinishedGoods,
            Packaging = source.Packaging
        };
    }

    private static ProductionEfficiencySeriesDto MapProductionEfficiencySeries(ProductionEfficiencySeriesReadModel source)
    {
        return new ProductionEfficiencySeriesDto
        {
            Line = source.Line,
            Points = source.Points.Select(MapChartPoint).ToList()
        };
    }
}