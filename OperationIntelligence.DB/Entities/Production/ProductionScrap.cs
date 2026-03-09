namespace OperationIntelligence.DB;

public class ProductionScrap : AuditableEntity
{
    public Guid ProductionOrderId { get; set; }
    public ProductionOrder ProductionOrder { get; set; } = default!;

    public Guid? ProductionExecutionId { get; set; }
    public ProductionExecution? ProductionExecution { get; set; }

    public Guid ProductId { get; set; }
    public Product Product { get; set; } = default!;

    public decimal ScrapQuantity { get; set; }

    public Guid UnitOfMeasureId { get; set; }
    public UnitOfMeasure UnitOfMeasure { get; set; } = default!;

    public ScrapReasonType Reason { get; set; }
    public string? ReasonDescription { get; set; }

    public DateTime ScrapDate { get; set; }

    public bool IsReworkable { get; set; }

    public string? Notes { get; set; }
}