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