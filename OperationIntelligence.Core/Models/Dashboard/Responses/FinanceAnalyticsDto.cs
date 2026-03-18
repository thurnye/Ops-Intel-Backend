namespace OperationIntelligence.Core;

public class FinanceAnalyticsDto
{
    public List<MultiSeriesPointDto> RevenueExpenseTrend { get; set; } = new();
    public List<PieSliceDto> ExpenseBreakdown { get; set; } = new();
}