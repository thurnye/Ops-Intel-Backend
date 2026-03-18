namespace OperationIntelligence.Core;

public class ShipmentAnalyticsDto
{
    public List<MultiSeriesPointDto> OnTimeVsDelayedTrend { get; set; } = new();
    public List<PieSliceDto> ShipmentStatusDistribution { get; set; } = new();
}