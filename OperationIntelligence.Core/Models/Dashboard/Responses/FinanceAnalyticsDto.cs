namespace OperationIntelligence.Core;

public class FinanceAnalyticsDto
{
    public List<MultiSeriesPointDto> RevenueExpenseTrend { get; set; } = new();
    public List<PieSliceDto> ExpenseBreakdown { get; set; } = new();
}

public class DashboardFinanceSectionDto
{
    public string SectionTitle { get; set; } = string.Empty;
    public string RevenueExpenseTitle { get; set; } = string.Empty;
    public string ExpenseBreakdownTitle { get; set; } = string.Empty;
    public DashboardLineChartDto RevenueExpenseTrend { get; set; } = new();
    public List<DashboardPieSliceDto> ExpenseBreakdown { get; set; } = new();
}

public class DashboardInventorySectionDto
{
    public string SectionTitle { get; set; } = string.Empty;
    public string LowStockTitle { get; set; } = string.Empty;
    public DashboardBarChartDto LowStockChart { get; set; } = new();
    public string InflowOutflowTitle { get; set; } = string.Empty;
    public DashboardLineChartDto InflowOutflowChart { get; set; } = new();
    public string WarehouseCompositionTitle { get; set; } = string.Empty;
    public DashboardBarChartDto WarehouseCompositionChart { get; set; } = new();
    public string InventoryMixTitle { get; set; } = string.Empty;
    public List<DashboardPieSliceDto> InventoryMix { get; set; } = new();
}

public class DashboardProductionSectionDto
{
    public string SectionTitle { get; set; } = string.Empty;
    public string EfficiencyTitle { get; set; } = string.Empty;
    public DashboardLineChartDto EfficiencyChart { get; set; } = new();
    public string StatusMixTitle { get; set; } = string.Empty;
    public List<DashboardPieSliceDto> StatusMix { get; set; } = new();
    public string PlannedVsActualTitle { get; set; } = string.Empty;
    public DashboardBarChartDto PlannedVsActualChart { get; set; } = new();
}

public class DashboardShipmentSectionDto
{
    public string SectionTitle { get; set; } = string.Empty;
    public string OnTimeVsDelayedTitle { get; set; } = string.Empty;
    public DashboardLineChartDto OnTimeVsDelayedChart { get; set; } = new();
    public string StatusDistributionTitle { get; set; } = string.Empty;
    public List<DashboardPieSliceDto> StatusDistribution { get; set; } = new();
    public string WeeklyOrdersVsShipmentsTitle { get; set; } = string.Empty;
    public DashboardBarChartDto WeeklyOrdersVsShipmentsChart { get; set; } = new();
    public string TeamTaskCompletionTitle { get; set; } = string.Empty;
    public List<DashboardProgressStatDto> TeamTaskCompletion { get; set; } = new();
    public string InventoryMixTitle { get; set; } = string.Empty;
    public List<DashboardPieSliceDto> InventoryMix { get; set; } = new();
}

public class DashboardProgressStatDto
{
    public string Label { get; set; } = string.Empty;
    public decimal Value { get; set; }
}