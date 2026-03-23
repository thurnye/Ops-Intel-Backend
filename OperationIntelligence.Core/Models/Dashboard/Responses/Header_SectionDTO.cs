namespace OperationIntelligence.Core;

public class DashboardHeaderDto
{
    public string Title { get; set; } = "Dashboard";
    public string Subtitle { get; set; } = string.Empty;
    public List<DashboardOptionDto> RangeOptions { get; set; } = new();
    public List<DashboardOptionDto> SiteOptions { get; set; } = new();
    public string RefreshLabel { get; set; } = "Refresh";
    public string ExportLabel { get; set; } = "Export";
}

public class DashboardSectionHeaderDto
{
    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
}

public class DashboardOptionDto
{
    public string Value { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
}