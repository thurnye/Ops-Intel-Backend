namespace OperationIntelligence.DB;

public class ChartPointReadModel
{
    public string Label { get; set; } = string.Empty;
    public decimal Value { get; set; }
}

public class MultiSeriesPointReadModel
{
    public string Label { get; set; } = string.Empty;
    public decimal Series1 { get; set; }
    public decimal Series2 { get; set; }
    public decimal? Series3 { get; set; }
}

public class PieSliceReadModel
{
    public string Label { get; set; } = string.Empty;
    public decimal Value { get; set; }
}

public class BusinessPerformanceReadModel
{
    public List<ChartPointReadModel> MonthlyRevenueTrend { get; set; } = new();
    public decimal OnTimeShipmentRate { get; set; }
    public decimal WarehouseCapacityUse { get; set; }
    public int ApprovalQueue { get; set; }
}

public class FinanceAnalyticsReadModel
{
    public List<MultiSeriesPointReadModel> RevenueExpenseTrend { get; set; } = new();
    public List<PieSliceReadModel> ExpenseBreakdown { get; set; } = new();
}

public class InventoryAnalyticsReadModel
{
    public List<ChartPointReadModel> TopLowStockItems { get; set; } = new();
    public List<MultiSeriesPointReadModel> InventoryInflowOutflow { get; set; } = new();
    public List<WarehouseStockCompositionReadModel> WarehouseStockComposition { get; set; } = new();
    public List<PieSliceReadModel> InventoryMixByCategory { get; set; } = new();
}

public class WarehouseStockCompositionReadModel
{
    public string Warehouse { get; set; } = string.Empty;
    public decimal RawMaterials { get; set; }
    public decimal FinishedGoods { get; set; }
    public decimal Packaging { get; set; }
}

public class ProductionAnalyticsReadModel
{
    public List<MultiSeriesPointReadModel> PlannedVsActualOutput { get; set; } = new();
    public List<PieSliceReadModel> ProductionJobStatusMix { get; set; } = new();
    public List<ProductionEfficiencySeriesReadModel> ProductionLineEfficiency { get; set; } = new();
}

public class ProductionEfficiencySeriesReadModel
{
    public string Line { get; set; } = string.Empty;
    public List<ChartPointReadModel> Points { get; set; } = new();
}

public class ShipmentAnalyticsReadModel
{
    public List<MultiSeriesPointReadModel> OnTimeVsDelayedTrend { get; set; } = new();
    public List<PieSliceReadModel> ShipmentStatusDistribution { get; set; } = new();
}