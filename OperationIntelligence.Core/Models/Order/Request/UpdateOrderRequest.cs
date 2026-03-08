namespace OperationIntelligence.Core;

public class UpdateOrderRequest
{
    public string? CustomerName { get; set; }
    public string? CustomerEmail { get; set; }
    public string? CustomerPhone { get; set; }
    public OrderPriority Priority { get; set; }
    public Guid? WarehouseId { get; set; }
    public DateTime? RequiredDateUtc { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? CustomerPurchaseOrderNumber { get; set; }
    public string? Notes { get; set; }
}
