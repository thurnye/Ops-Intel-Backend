namespace OperationIntelligence.Core;

public class DashboardAlertDto
{
    public string Title { get; set; } = string.Empty;
    public string Detail { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
}
public class DashboardAttentionRequiredSectionDto
{
    public string Title { get; set; } = string.Empty;
    public List<DashboardAlertDto> Alerts { get; set; } = new();
    public string QuickActionsTitle { get; set; } = string.Empty;
    public List<DashboardQuickActionDto> QuickActions { get; set; } = new();
}

public class DashboardQuickActionDto
{
    public string Id { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string IconKey { get; set; } = string.Empty;
    public string Variant { get; set; } = "outlined";
}