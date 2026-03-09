namespace OperationIntelligence.Core.Models.Production.Responses;

public class ProductionMaterialConsumptionResponse
{
    public Guid Id { get; set; }
    public Guid ProductionMaterialIssueId { get; set; }
    public Guid? ProductionExecutionId { get; set; }
    public decimal ConsumedQuantity { get; set; }
    public DateTime ConsumptionDate { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public string? CreatedBy { get; set; }
}
