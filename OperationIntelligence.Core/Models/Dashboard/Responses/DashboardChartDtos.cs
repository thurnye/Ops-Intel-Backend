namespace OperationIntelligence.Core;

public class ChartPointDto
{
    public string Label { get; set; } = string.Empty;
    public decimal Value { get; set; }
}

public class MultiSeriesPointDto
{
    public string Label { get; set; } = string.Empty;
    public decimal Series1 { get; set; }
    public decimal Series2 { get; set; }
    public decimal? Series3 { get; set; }
}

public class PieSliceDto
{
    public string Label { get; set; } = string.Empty;
    public decimal Value { get; set; }
}
