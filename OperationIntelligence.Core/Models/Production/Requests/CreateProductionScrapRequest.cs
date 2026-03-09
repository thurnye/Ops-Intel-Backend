using OperationIntelligence.DB;

namespace OperationIntelligence.Core.Models.Production.Requests;

public class CreateProductionScrapRequest
{
    public Guid ProductionOrderId { get; set; }
    public Guid? ProductionExecutionId { get; set; }
    public Guid ProductId { get; set; }
    public decimal ScrapQuantity { get; set; }
    public Guid UnitOfMeasureId { get; set; }
    public ScrapReasonType Reason { get; set; }
    public string? ReasonDescription { get; set; }
    public DateTime ScrapDate { get; set; }
    public bool IsReworkable { get; set; }
    public string? Notes { get; set; }
}
