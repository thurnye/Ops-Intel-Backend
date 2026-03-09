namespace OperationIntelligence.Core.Models.Production.Responses;

public class ProductionMaterialIssueResponse
{
    public Guid Id { get; set; }
    public Guid ProductionOrderId { get; set; }
    public Guid MaterialProductId { get; set; }
    public string? MaterialProductName { get; set; }
    public string? MaterialProductSku { get; set; }
    public Guid WarehouseId { get; set; }
    public string? WarehouseName { get; set; }
    public decimal PlannedQuantity { get; set; }
    public decimal IssuedQuantity { get; set; }
    public decimal ReturnedQuantity { get; set; }
    public Guid UnitOfMeasureId { get; set; }
    public string? UnitOfMeasureName { get; set; }
    public string? BatchNumber { get; set; }
    public string? LotNumber { get; set; }
    public DateTime IssueDate { get; set; }
    public Guid? StockMovementId { get; set; }
    public string? Notes { get; set; }
    public List<ProductionMaterialConsumptionResponse> Consumptions { get; set; } = new();
    public DateTime CreatedAtUtc { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public string? UpdatedBy { get; set; }
}
