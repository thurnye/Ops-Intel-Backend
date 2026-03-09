using OperationIntelligence.DB;

namespace OperationIntelligence.Core.Models.Production.Responses;

public class ProductionOrderSummaryResponse
{
    public Guid Id { get; set; }
    public string ProductionOrderNumber { get; set; } = string.Empty;
    public Guid ProductId { get; set; }
    public string? ProductName { get; set; }
    public string? ProductSku { get; set; }
    public decimal PlannedQuantity { get; set; }
    public decimal ProducedQuantity { get; set; }
    public decimal RemainingQuantity { get; set; }
    public Guid UnitOfMeasureId { get; set; }
    public string? UnitOfMeasureName { get; set; }
    public Guid WarehouseId { get; set; }
    public string? WarehouseName { get; set; }
    public DateTime PlannedStartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
    public ProductionOrderStatus Status { get; set; }
    public ProductionPriority Priority { get; set; }
    public bool IsReleased { get; set; }
    public bool IsClosed { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
