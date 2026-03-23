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


public class DashboardSummarySnapshotDto
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string IconKey { get; set; } = string.Empty;
    public string AccentTone { get; set; } = string.Empty;
    public string PrimaryLabel { get; set; } = string.Empty;
    public string PrimaryValue { get; set; } = string.Empty;
    public List<DashboardLabelValueDto> Stats { get; set; } = new();
}

public class DashboardLabelValueDto
{
    public string Label { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

public class DashboardWorkflowSectionDto
{
    public string Title { get; set; } = string.Empty;
    public List<DashboardWorkflowStepDto> Steps { get; set; } = new();
}

public class DashboardWorkflowStepDto
{
    public string Label { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal Progress { get; set; }
    public string Color { get; set; } = string.Empty;
}

public class DashboardActivityFeedSectionDto
{
    public string Title { get; set; } = string.Empty;
    public List<DashboardActivityItemDto> Items { get; set; } = new();
    public DashboardInsightDto Insight { get; set; } = new();
}

public class DashboardActivityItemDto
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public string Time { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
}

public class DashboardInsightDto
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string IconKey { get; set; } = string.Empty;
}

public class DashboardTablesSectionDto
{
    public string RecentOrdersTitle { get; set; } = string.Empty;
    public string LowStockItemsTitle { get; set; } = string.Empty;
    public List<RecentOrderDto> RecentOrders { get; set; } = new();
    public List<LowStockItemDto> LowStockItems { get; set; } = new();
}