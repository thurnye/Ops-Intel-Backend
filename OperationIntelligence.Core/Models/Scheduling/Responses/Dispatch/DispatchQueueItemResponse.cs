namespace OperationIntelligence.Core.Models.Scheduling.Responses.Dispatch;

public class DispatchQueueItemResponse
{
    public Guid Id { get; set; }
    public Guid ScheduleOperationId { get; set; }
    public Guid WorkCenterId { get; set; }
    public string WorkCenterName { get; set; } = string.Empty;
    public Guid? MachineId { get; set; }
    public string? MachineName { get; set; }
    public int QueuePosition { get; set; }
    public int PriorityScore { get; set; }
    public int Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public DateTime? ReleasedAtUtc { get; set; }
    public DateTime? AcknowledgedAtUtc { get; set; }
    public string? DispatchNotes { get; set; }
    public bool IsActive { get; set; }
}
