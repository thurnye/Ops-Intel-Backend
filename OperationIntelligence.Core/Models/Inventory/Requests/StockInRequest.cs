namespace OperationIntelligence.Core;
public class StockInRequest
{
    public Guid ProductId { get; set; }
    public Guid WarehouseId { get; set; }

    public decimal Quantity { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }
}
