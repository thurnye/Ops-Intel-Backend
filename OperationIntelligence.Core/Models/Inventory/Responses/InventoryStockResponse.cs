namespace OperationIntelligence.Core;

public class InventoryStockResponse
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Guid WarehouseId { get; set; }

    public string WarehouseName { get; set; } = string.Empty;

    public decimal QuantityOnHand { get; set; }
    public decimal QuantityReserved { get; set; }
    public decimal QuantityAvailable { get; set; }
    public decimal QuantityDamaged { get; set; }
}
