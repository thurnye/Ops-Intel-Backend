using OperationIntelligence.DB;

namespace OperationIntelligence.Core.Models.Production.Responses;

public class ProductionExecutionSummaryResponse
{
    public Guid Id { get; set; }
    public Guid ProductionOrderId { get; set; }
    public string? ProductionOrderNumber { get; set; }
    public Guid WorkCenterId { get; set; }
    public string? WorkCenterName { get; set; }
    public Guid? MachineId { get; set; }
    public string? MachineName { get; set; }
    public decimal PlannedQuantity { get; set; }
    public decimal CompletedQuantity { get; set; }
    public decimal ScrapQuantity { get; set; }
    public DateTime PlannedStartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
    public ExecutionStatus Status { get; set; }
}
