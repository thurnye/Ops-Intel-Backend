namespace OperationIntelligence.DB;

public class ProductionDowntime : AuditableEntity
{
    public Guid ProductionExecutionId { get; set; }
    public ProductionExecution ProductionExecution { get; set; } = default!;

    public DowntimeReasonType Reason { get; set; }
    public string? ReasonDescription { get; set; }

    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    public decimal DurationMinutes { get; set; }

    public bool IsPlanned { get; set; }

    public string? Notes { get; set; }
}