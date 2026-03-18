namespace OperationIntelligence.Core;

public class WorkflowPipelineDto
{
    public string Label { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal Progress { get; set; }
}

public class ActivityFeedDto
{
    public string Text { get; set; } = string.Empty;
    public DateTime TimeUtc { get; set; }
    public string Type { get; set; } = string.Empty;
}

public class RecentOrderDto
{
    public string OrderNo { get; set; } = string.Empty;
    public string Customer { get; set; } = string.Empty;
    public string Module { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public string Warehouse { get; set; } = string.Empty;
}

public class LowStockItemDto
{
    public string Sku { get; set; } = string.Empty;
    public string Item { get; set; } = string.Empty;
    public string Warehouse { get; set; } = string.Empty;
    public decimal Stock { get; set; }
    public decimal ReorderLevel { get; set; }
    public string Status { get; set; } = string.Empty;
}
