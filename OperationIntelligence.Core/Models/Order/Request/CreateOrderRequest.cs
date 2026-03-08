namespace OperationIntelligence.Core;

public class CreateOrderRequest
{
    public Guid? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerEmail { get; set; }
    public string? CustomerPhone { get; set; }

    public OrderType OrderType { get; set; }
    public OrderPriority Priority { get; set; }
    public OrderChannel Channel { get; set; }
    public Guid? WarehouseId { get; set; }

    public DateTime? RequiredDateUtc { get; set; }
    public string CurrencyCode { get; set; } = "CAD";
    public string? ReferenceNumber { get; set; }
    public string? CustomerPurchaseOrderNumber { get; set; }
    public string? Notes { get; set; }

    public List<CreateOrderItemRequest> Items { get; set; } = new();
    public List<CreateOrderAddressRequest> Addresses { get; set; } = new();
}