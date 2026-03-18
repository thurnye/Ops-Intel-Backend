namespace OperationIntelligence.DB;

public class WorkflowPipelineReadModel
{
    public string Label { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal Progress { get; set; }
}

public class ActivityFeedReadModel
{
    public string Text { get; set; } = string.Empty;
    public DateTime TimeUtc { get; set; }
    public string Type { get; set; } = string.Empty;
}

public class WorkforceSummaryReadModel
{
    public int ActiveStaff { get; set; }
    public decimal ShiftCoverage { get; set; }
    public int OpenPositions { get; set; }
}

public class ProcurementSummaryReadModel
{
    public int OpenPurchaseOrders { get; set; }
    public int AwaitingApproval { get; set; }
    public decimal SupplierSlaMet { get; set; }
}

public class WarehouseSummaryReadModel
{
    public int WarehousesActive { get; set; }
    public decimal AveragePickAccuracy { get; set; }
    public decimal CrossDockUtilization { get; set; }
}

public class RecentOrderReadModel
{
    public string OrderNo { get; set; } = string.Empty;
    public string Customer { get; set; } = string.Empty;
    public string Module { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public string Warehouse { get; set; } = string.Empty;
}

public class LowStockItemReadModel
{
    public string Sku { get; set; } = string.Empty;
    public string Item { get; set; } = string.Empty;
    public string Warehouse { get; set; } = string.Empty;
    public decimal Stock { get; set; }
    public decimal ReorderLevel { get; set; }
    public string Status { get; set; } = string.Empty;
}