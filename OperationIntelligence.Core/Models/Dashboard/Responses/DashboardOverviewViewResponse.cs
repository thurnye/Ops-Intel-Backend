namespace OperationIntelligence.Core;

public class DashboardOverviewViewResponse
{
    public DashboardHeaderDto Header { get; set; } = new();
    public DashboardSectionHeaderDto AnalyticsHeader { get; set; } = new();

    public List<DashboardKpiCardDto> Kpis { get; set; } = new();

    public DashboardBusinessPerformanceSectionDto BusinessPerformance { get; set; } = new();
    public DashboardAttentionRequiredSectionDto AttentionRequired { get; set; } = new();

    public DashboardFinanceSectionDto Finance { get; set; } = new();
    public DashboardModuleHealthSectionDto ModuleHealth { get; set; } = new();
    public DashboardInventorySectionDto Inventory { get; set; } = new();
    public DashboardProductionSectionDto Production { get; set; } = new();
    public DashboardShipmentSectionDto Shipments { get; set; } = new();

    public List<DashboardSummarySnapshotDto> SummarySnapshots { get; set; } = new();

    public DashboardWorkflowSectionDto Workflow { get; set; } = new();
    public DashboardActivityFeedSectionDto ActivityFeed { get; set; } = new();
    public DashboardTablesSectionDto Tables { get; set; } = new();
}