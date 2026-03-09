using OperationIntelligence.DB;

namespace OperationIntelligence.Core.Models.Production.Requests;

public class UpdateProductionOrderRequest
{
    public Guid ProductId { get; set; }
    public decimal PlannedQuantity { get; set; }
    public Guid UnitOfMeasureId { get; set; }
    public Guid? BillOfMaterialId { get; set; }
    public Guid? RoutingId { get; set; }
    public Guid WarehouseId { get; set; }
    public DateTime PlannedStartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
    public ProductionPriority Priority { get; set; }
    public string? BatchNumber { get; set; }
    public string? LotNumber { get; set; }
    public string? Notes { get; set; }
}
