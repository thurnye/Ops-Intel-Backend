namespace OperationIntelligence.Core.Models.Production.Responses;

public class ProductionOutputResponse
{
    public Guid Id { get; set; }
    public Guid ProductionOrderId { get; set; }
    public Guid ProductId { get; set; }
    public string? ProductName { get; set; }
    public string? ProductSku { get; set; }
    public Guid WarehouseId { get; set; }
    public string? WarehouseName { get; set; }
    public decimal QuantityProduced { get; set; }
    public Guid UnitOfMeasureId { get; set; }
    public string? UnitOfMeasureName { get; set; }
    public string? BatchNumber { get; set; }
    public string? LotNumber { get; set; }
    public DateTime OutputDate { get; set; }
    public Guid? StockMovementId { get; set; }
    public bool IsFinalOutput { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public string? UpdatedBy { get; set; }
}
