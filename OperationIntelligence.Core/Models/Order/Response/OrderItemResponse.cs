namespace OperationIntelligence.Core;

public class OrderItemResponse
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductNameSnapshot { get; set; } = default!;
    public string ProductSkuSnapshot { get; set; } = default!;
    public decimal QuantityOrdered { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal LineTotal { get; set; }
}