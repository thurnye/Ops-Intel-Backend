namespace OperationIntelligence.Core.Models.Scheduling.Requests.Revision;

public class CreateScheduleRevisionRequest
{
    public Guid SchedulePlanId { get; set; }
    public int RevisionNo { get; set; }

    public string RevisionType { get; set; } = string.Empty;
    public string ChangeSummary { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;

    public DateTime RevisedAtUtc { get; set; }
    public string? SnapshotJson { get; set; }
}
