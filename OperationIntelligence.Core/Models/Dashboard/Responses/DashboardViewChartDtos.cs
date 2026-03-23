namespace OperationIntelligence.Core;

public class DashboardChartSeriesDto
{
    public string Label { get; set; } = string.Empty;
    public List<decimal> Data { get; set; } = new();
    public string? Stack { get; set; }
}

public class DashboardLineChartDto
{
    public List<string> Labels { get; set; } = new();
    public List<DashboardChartSeriesDto> Series { get; set; } = new();
}

public class DashboardBarChartDto
{
    public List<string> Labels { get; set; } = new();
    public List<DashboardChartSeriesDto> Series { get; set; } = new();
    public string? Layout { get; set; }
}

public class DashboardPieSliceDto
{
    public int Id { get; set; }
    public decimal Value { get; set; }
    public string Label { get; set; } = string.Empty;
}