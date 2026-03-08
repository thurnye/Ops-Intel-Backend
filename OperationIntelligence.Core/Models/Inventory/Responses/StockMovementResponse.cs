using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class StockMovementResponse
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;

    public Guid WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;

    public StockMovementType MovementType { get; set; }
    public decimal Quantity { get; set; }
    public decimal QuantityBefore { get; set; }
    public decimal QuantityAfter { get; set; }

    public string? ReferenceNumber { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }

    public DateTime MovementDateUtc { get; set; }
}
