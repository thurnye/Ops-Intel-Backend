namespace OperationIntelligence.Core.Models.Scheduling.Requests.Dispatch;

public class ReleaseDispatchQueueItemRequest
{
    public DateTime ReleasedAtUtc { get; set; }
    public string? DispatchNotes { get; set; }
}
