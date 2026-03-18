namespace OperationIntelligence.Core;

public class ProductionAnalyticsDto
{
    public List<MultiSeriesPointDto> PlannedVsActualOutput { get; set; } = new();
    public List<PieSliceDto> ProductionJobStatusMix { get; set; } = new();
    public List<ProductionEfficiencySeriesDto> ProductionLineEfficiency { get; set; } = new();
}

public class ProductionEfficiencySeriesDto
{
    public string Line { get; set; } = string.Empty;
    public List<ChartPointDto> Points { get; set; } = new();
}