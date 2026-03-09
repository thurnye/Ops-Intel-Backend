using OperationIntelligence.DB;

namespace OperationIntelligence.Core.Models.Production.Requests;

public class CreateProductionOrderRequest
{
    public string ProductionOrderNumber { get; set; } = string.Empty;
    public Guid ProductId { get; set; }
    public decimal PlannedQuantity { get; set; }
    public Guid UnitOfMeasureId { get; set; }
    public Guid? BillOfMaterialId { get; set; }
    public Guid? RoutingId { get; set; }
    public Guid WarehouseId { get; set; }
    public DateTime PlannedStartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
    public ProductionPriority Priority { get; set; } = ProductionPriority.Medium;
    public ProductionSourceType SourceType { get; set; } = ProductionSourceType.Manual;
    public Guid? SourceReferenceId { get; set; }
    public string? BatchNumber { get; set; }
    public string? LotNumber { get; set; }
    public string? Notes { get; set; }
}
