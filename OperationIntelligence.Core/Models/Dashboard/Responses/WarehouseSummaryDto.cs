namespace OperationIntelligence.Core;

public class WarehouseSummaryDto
{
    /// <summary>
    /// Total active warehouses in the system
    /// </summary>
    public int WarehousesActive { get; set; }

    /// <summary>
    /// Picking accuracy percentage (0–100)
    /// </summary>
    public decimal AveragePickAccuracy { get; set; }

    /// <summary>
    /// Cross-docking utilization percentage (0–100)
    /// </summary>
    public decimal CrossDockUtilization { get; set; }
}