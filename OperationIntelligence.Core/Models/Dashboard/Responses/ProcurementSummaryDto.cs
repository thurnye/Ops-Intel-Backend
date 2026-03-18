namespace OperationIntelligence.Core;

public class ProcurementSummaryDto
{
    /// <summary>
    /// Total number of open purchase orders (not fulfilled/cancelled)
    /// </summary>
    public int OpenPurchaseOrders { get; set; }

    /// <summary>
    /// Purchase orders awaiting approval
    /// </summary>
    public int AwaitingApproval { get; set; }

    /// <summary>
    /// Supplier SLA compliance percentage (0–100)
    /// </summary>
    public decimal SupplierSlaMet { get; set; }
}