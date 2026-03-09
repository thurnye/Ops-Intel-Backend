namespace OperationIntelligence.Core.Models.Scheduling.Requests.ScheduleOperation;

public class StartScheduleOperationRequest
{
    public Guid? ActualShiftId { get; set; }
    public Guid? ProductionExecutionId { get; set; }
    public DateTime ActualStartUtc { get; set; }
    public string? Notes { get; set; }
}
