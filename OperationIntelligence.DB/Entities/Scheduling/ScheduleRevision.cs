namespace OperationIntelligence.DB;

public class ScheduleRevision : AuditableEntity
{
    public Guid SchedulePlanId { get; set; }
    public SchedulePlan SchedulePlan { get; set; } = default!;

    public int RevisionNo { get; set; }

    public string RevisionType { get; set; } = string.Empty;
    public string ChangeSummary { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;

    public DateTime RevisedAtUtc { get; set; }
    public string? SnapshotJson { get; set; }
}
