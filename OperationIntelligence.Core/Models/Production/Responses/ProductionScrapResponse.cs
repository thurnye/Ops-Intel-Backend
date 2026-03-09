using OperationIntelligence.DB;

namespace OperationIntelligence.Core.Models.Production.Responses;

public class ProductionScrapResponse
{
    public Guid Id { get; set; }
    public Guid ProductionOrderId { get; set; }
    public Guid? ProductionExecutionId { get; set; }
    public Guid ProductId { get; set; }
    public string? ProductName { get; set; }
    public string? ProductSku { get; set; }
    public decimal ScrapQuantity { get; set; }
    public Guid UnitOfMeasureId { get; set; }
    public string? UnitOfMeasureName { get; set; }
    public ScrapReasonType Reason { get; set; }
    public string? ReasonDescription { get; set; }
    public DateTime ScrapDate { get; set; }
    public bool IsReworkable { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public string? UpdatedBy { get; set; }
}
