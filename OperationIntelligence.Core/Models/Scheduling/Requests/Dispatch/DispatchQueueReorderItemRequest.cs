namespace OperationIntelligence.Core.Models.Scheduling.Requests.Dispatch;

public class DispatchQueueReorderItemRequest
{
    public Guid DispatchQueueItemId { get; set; }
    public int QueuePosition { get; set; }
}
