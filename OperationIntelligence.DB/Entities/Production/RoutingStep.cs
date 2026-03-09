namespace OperationIntelligence.DB;

public class RoutingStep : AuditableEntity
{
    public Guid RoutingId { get; set; }
    public Routing Routing { get; set; } = default!;

    public int Sequence { get; set; }

    public string OperationCode { get; set; } = string.Empty;
    public string OperationName { get; set; } = string.Empty;

    public Guid WorkCenterId { get; set; }
    public WorkCenter WorkCenter { get; set; } = default!;

    public decimal SetupTimeMinutes { get; set; }
    public decimal RunTimeMinutesPerUnit { get; set; }
    public decimal QueueTimeMinutes { get; set; }
    public decimal WaitTimeMinutes { get; set; }
    public decimal MoveTimeMinutes { get; set; }

    public int RequiredOperators { get; set; }

    public bool IsOutsourced { get; set; }
    public bool IsQualityCheckpointRequired { get; set; }

    public string? Instructions { get; set; }
    public string? Notes { get; set; }

    public ICollection<ProductionExecution> ProductionExecutions { get; set; } = new List<ProductionExecution>();
}