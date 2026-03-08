namespace OperationIntelligence.Core;

public class CreateOrderItemRequest
{
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public Guid? UnitOfMeasureId { get; set; }
    public decimal QuantityOrdered { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public string? Remarks { get; set; }
    public int SortOrder { get; set; }
}