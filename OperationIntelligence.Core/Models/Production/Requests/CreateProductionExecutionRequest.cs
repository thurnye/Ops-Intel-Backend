namespace OperationIntelligence.Core.Models.Production.Requests;

public class CreateProductionExecutionRequest
{
    public Guid ProductionOrderId { get; set; }
    public Guid? RoutingStepId { get; set; }
    public Guid WorkCenterId { get; set; }
    public Guid? MachineId { get; set; }
    public decimal PlannedQuantity { get; set; }
    public DateTime PlannedStartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
    public string? Remarks { get; set; }
}
