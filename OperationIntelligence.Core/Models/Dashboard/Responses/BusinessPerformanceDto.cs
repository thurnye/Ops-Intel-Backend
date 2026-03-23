namespace OperationIntelligence.Core;

public class BusinessPerformanceDto
{
    public List<ChartPointDto> MonthlyRevenueTrend { get; set; } = new();
    public decimal OnTimeShipmentRate { get; set; }
    public decimal WarehouseCapacityUse { get; set; }
    public int ApprovalQueue { get; set; }
}

public class DashboardBusinessPerformanceSectionDto
{
    public string Title { get; set; } = string.Empty;
    public List<string> MetricChips { get; set; } = new();
    public string RevenueTrendTitle { get; set; } = string.Empty;
    public DashboardLineChartDto RevenueTrend { get; set; } = new();
    public List<DashboardProgressCardDto> ProgressCards { get; set; } = new();
}

public class DashboardProgressCardDto
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public decimal Progress { get; set; }
    public string Color { get; set; } = string.Empty;
    public string? Description { get; set; }
}