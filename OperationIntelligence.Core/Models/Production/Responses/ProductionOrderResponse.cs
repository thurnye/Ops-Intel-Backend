using OperationIntelligence.DB;

namespace OperationIntelligence.Core.Models.Production.Responses;

public class ProductionOrderResponse
{
    public Guid Id { get; set; }
    public string ProductionOrderNumber { get; set; } = string.Empty;
    public Guid ProductId { get; set; }
    public string? ProductName { get; set; }
    public string? ProductSku { get; set; }
    public decimal PlannedQuantity { get; set; }
    public decimal ProducedQuantity { get; set; }
    public decimal ScrapQuantity { get; set; }
    public decimal RemainingQuantity { get; set; }
    public Guid UnitOfMeasureId { get; set; }
    public string? UnitOfMeasureName { get; set; }
    public Guid? BillOfMaterialId { get; set; }
    public string? BillOfMaterialCode { get; set; }
    public string? BillOfMaterialName { get; set; }
    public Guid? RoutingId { get; set; }
    public string? RoutingCode { get; set; }
    public string? RoutingName { get; set; }
    public Guid WarehouseId { get; set; }
    public string? WarehouseName { get; set; }
    public DateTime PlannedStartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public DateTime? ActualEndDate { get; set; }
    public ProductionOrderStatus Status { get; set; }
    public ProductionPriority Priority { get; set; }
    public ProductionSourceType SourceType { get; set; }
    public Guid? SourceReferenceId { get; set; }
    public string? BatchNumber { get; set; }
    public string? LotNumber { get; set; }
    public string? Notes { get; set; }
    public decimal EstimatedMaterialCost { get; set; }
    public decimal EstimatedLaborCost { get; set; }
    public decimal EstimatedOverheadCost { get; set; }
    public decimal ActualMaterialCost { get; set; }
    public decimal ActualLaborCost { get; set; }
    public decimal ActualOverheadCost { get; set; }
    public bool IsReleased { get; set; }
    public bool IsClosed { get; set; }
    public List<ProductionExecutionResponse> Executions { get; set; } = new();
    public List<ProductionMaterialIssueResponse> MaterialIssues { get; set; } = new();
    public List<ProductionOutputResponse> Outputs { get; set; } = new();
    public List<ProductionScrapResponse> Scraps { get; set; } = new();
    public List<ProductionQualityCheckResponse> QualityChecks { get; set; } = new();
    public DateTime CreatedAtUtc { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public string? UpdatedBy { get; set; }
}
