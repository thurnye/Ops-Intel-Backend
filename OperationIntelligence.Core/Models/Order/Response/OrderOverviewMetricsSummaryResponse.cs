namespace OperationIntelligence.Core;

public class OrderOverviewMetricsSummaryResponse
{
    public int TotalOrders { get; set; }
    public int AwaitingAction { get; set; }
    public int Processing { get; set; }
    public int ShippedOrDelivered { get; set; }
}
