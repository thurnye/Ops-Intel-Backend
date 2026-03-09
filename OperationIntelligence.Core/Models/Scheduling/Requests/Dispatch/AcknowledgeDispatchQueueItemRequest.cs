namespace OperationIntelligence.Core.Models.Scheduling.Requests.Dispatch;

public class AcknowledgeDispatchQueueItemRequest
{
    public DateTime AcknowledgedAtUtc { get; set; }
    public string? DispatchNotes { get; set; }
}
