using OperationIntelligence.DB;

namespace OperationIntelligence.Core.Models.Production.Responses;

public class ProductionExecutionResponse
{
    public Guid Id { get; set; }
    public Guid ProductionOrderId { get; set; }
    public string? ProductionOrderNumber { get; set; }
    public Guid? RoutingStepId { get; set; }
    public int? RoutingStepSequence { get; set; }
    public string? OperationCode { get; set; }
    public string? OperationName { get; set; }
    public Guid WorkCenterId { get; set; }
    public string? WorkCenterName { get; set; }
    public Guid? MachineId { get; set; }
    public string? MachineName { get; set; }
    public string? MachineCode { get; set; }
    public decimal PlannedQuantity { get; set; }
    public decimal CompletedQuantity { get; set; }
    public decimal ScrapQuantity { get; set; }
    public DateTime PlannedStartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public DateTime? ActualEndDate { get; set; }
    public decimal ActualSetupTimeMinutes { get; set; }
    public decimal ActualRunTimeMinutes { get; set; }
    public decimal ActualDowntimeMinutes { get; set; }
    public ExecutionStatus Status { get; set; }
    public string? Remarks { get; set; }
    public List<ProductionMaterialConsumptionResponse> MaterialConsumptions { get; set; } = new();
    public List<ProductionLaborLogResponse> LaborLogs { get; set; } = new();
    public List<ProductionDowntimeResponse> Downtimes { get; set; } = new();
    public List<ProductionScrapResponse> Scraps { get; set; } = new();
    public List<ProductionQualityCheckResponse> QualityChecks { get; set; } = new();
    public DateTime CreatedAtUtc { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public string? UpdatedBy { get; set; }
}
