namespace OperationIntelligence.Core;

public class BusinessPerformanceDto
{
    public List<ChartPointDto> MonthlyRevenueTrend { get; set; } = new();
    public decimal OnTimeShipmentRate { get; set; }
    public decimal WarehouseCapacityUse { get; set; }
    public int ApprovalQueue { get; set; }
}
