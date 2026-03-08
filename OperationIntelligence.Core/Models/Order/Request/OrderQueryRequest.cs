namespace OperationIntelligence.Core;

public class OrderQueryRequest
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SearchTerm { get; set; }
    public OrderStatus? Status { get; set; }
    public OrderType? OrderType { get; set; }
    public Guid? WarehouseId { get; set; }
}