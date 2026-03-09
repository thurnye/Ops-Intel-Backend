namespace OperationIntelligence.Core.Models.Production.Requests;

public class CreateProductionMaterialConsumptionRequest
{
    public Guid ProductionMaterialIssueId { get; set; }
    public Guid? ProductionExecutionId { get; set; }
    public decimal ConsumedQuantity { get; set; }
    public DateTime ConsumptionDate { get; set; }
    public string? Notes { get; set; }
}
