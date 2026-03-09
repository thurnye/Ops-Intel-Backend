namespace OperationIntelligence.Core.Models.Production.Requests;

public class CreateProductionMaterialIssueRequest
{
    public Guid ProductionOrderId { get; set; }
    public Guid MaterialProductId { get; set; }
    public Guid WarehouseId { get; set; }
    public decimal PlannedQuantity { get; set; }
    public decimal IssuedQuantity { get; set; }
    public Guid UnitOfMeasureId { get; set; }
    public string? BatchNumber { get; set; }
    public string? LotNumber { get; set; }
    public DateTime IssueDate { get; set; }
    public string? Notes { get; set; }
}
