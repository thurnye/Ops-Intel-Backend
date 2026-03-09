namespace OperationIntelligence.Core.Models.Scheduling.Requests.Dispatch;

public class CreateDispatchQueueItemRequest
{
    public Guid ScheduleOperationId { get; set; }
    public Guid WorkCenterId { get; set; }
    public Guid? MachineId { get; set; }

    public int QueuePosition { get; set; }
    public int PriorityScore { get; set; }

    public string? DispatchNotes { get; set; }
}
