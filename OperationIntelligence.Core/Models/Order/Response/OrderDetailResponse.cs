namespace OperationIntelligence.Core;

public class OrderDetailResponse : OrderResponse
{
    public string? CustomerName { get; set; }
    public string? CustomerEmail { get; set; }
    public string? CustomerPhone { get; set; }
    public OrderType OrderType { get; set; }
    public OrderPriority Priority { get; set; }
    public OrderChannel Channel { get; set; }
    public Guid? WarehouseId { get; set; }
    public DateTime OrderDateUtc { get; set; }
    public DateTime? RequiredDateUtc { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? CustomerPurchaseOrderNumber { get; set; }
    public string? Notes { get; set; }

    public List<OrderItemResponse> Items { get; set; } = new();
    public List<OrderAddressResponse> Addresses { get; set; } = new();
    public List<OrderImageResponse> Images { get; set; } = new();
    public List<OrderNoteResponse> NotesList { get; set; } = new();
    public List<OrderStatusHistoryResponse> StatusHistory { get; set; } = new();
    public List<OrderPaymentResponse> Payments { get; set; } = new();
}