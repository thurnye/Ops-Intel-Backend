namespace OperationIntelligence.Core;

public class DashboardModuleHealthDto
{
    public string Module { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
}

public class DashboardModuleHealthSectionDto
{
    public List<DashboardModuleHealthCardDto> Cards { get; set; } = new();
}

public class DashboardModuleHealthCardDto
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
    public string IconKey { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
}
