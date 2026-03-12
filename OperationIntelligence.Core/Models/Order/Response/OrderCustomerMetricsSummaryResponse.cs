namespace OperationIntelligence.Core;

public class OrderCustomerMetricsSummaryResponse
{
    public int TotalCustomers { get; set; }
    public int RepeatCustomers { get; set; }
    public decimal TotalValue { get; set; }
    public decimal AverageCustomerValue { get; set; }
}
