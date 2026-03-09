namespace OperationIntelligence.DB;

public class ScheduleAuditLog : AuditableEntity
{
    public string EntityName { get; set; } = string.Empty;
    public Guid EntityId { get; set; }

    public string ActionType { get; set; } = string.Empty;

    public string ChangedFieldsJson { get; set; } = string.Empty;
    public string? OldValuesJson { get; set; }
    public string? NewValuesJson { get; set; }

    public string Source { get; set; } = string.Empty;
    public string? Reason { get; set; }

    public DateTime PerformedAtUtc { get; set; }
    public string? CorrelationId { get; set; }
}
