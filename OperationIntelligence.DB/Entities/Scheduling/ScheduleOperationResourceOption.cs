namespace OperationIntelligence.DB;

public class ScheduleOperationResourceOption : AuditableEntity
{
    public Guid ScheduleOperationId { get; set; }
    public ScheduleOperation ScheduleOperation { get; set; } = default!;

    public Guid ResourceId { get; set; }
    public ResourceType ResourceType { get; set; }

    public bool IsPrimaryOption { get; set; }
    public int PreferenceRank { get; set; }
    public decimal EfficiencyFactor { get; set; } = 1m;
    public decimal SetupPenaltyMinutes { get; set; }

    public bool IsActive { get; set; } = true;
}
