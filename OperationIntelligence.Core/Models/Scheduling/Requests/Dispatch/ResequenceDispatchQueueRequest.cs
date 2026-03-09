namespace OperationIntelligence.Core.Models.Scheduling.Requests.Dispatch;

public class ResequenceDispatchQueueRequest
{
    public Guid WorkCenterId { get; set; }
    public Guid? MachineId { get; set; }
    public List<DispatchQueueReorderItemRequest> Items { get; set; } = new();
    public string Reason { get; set; } = string.Empty;
}
