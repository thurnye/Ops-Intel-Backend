namespace OperationIntelligence.DB;

public class InventoryStock : AuditableEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = default!;

    public Guid WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = default!;

    public decimal QuantityOnHand { get; set; }
    public decimal QuantityReserved { get; set; }
    public decimal QuantityAvailable { get; set; }
    public decimal QuantityDamaged { get; set; }

    public DateTime? LastStockUpdatedAtUtc { get; set; }
}
