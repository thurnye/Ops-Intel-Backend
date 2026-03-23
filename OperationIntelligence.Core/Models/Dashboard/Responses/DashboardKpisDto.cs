namespace OperationIntelligence.Core;

public class DashboardKpisDto
{
    public decimal TotalRevenue { get; set; }
    public int OrdersInProgress { get; set; }
    public decimal ProductionEfficiency { get; set; }
    public decimal InventoryValue { get; set; }
    public int ShipmentsPending { get; set; }
    public int CriticalAlerts { get; set; }
}

public class DashboardKpiCardDto
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Change { get; set; } = string.Empty;
    public string Subtext { get; set; } = string.Empty;
    public string IconKey { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string Direction { get; set; } = "up";
}