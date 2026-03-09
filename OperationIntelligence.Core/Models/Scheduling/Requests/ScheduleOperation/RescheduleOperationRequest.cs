namespace OperationIntelligence.Core.Models.Scheduling.Requests.ScheduleOperation;

public class RescheduleOperationRequest
{
    public Guid? NewWorkCenterId { get; set; }
    public Guid? NewMachineId { get; set; }
    public Guid? NewPlannedShiftId { get; set; }

    public DateTime NewPlannedStartUtc { get; set; }
    public DateTime NewPlannedEndUtc { get; set; }

    public int? NewPriorityScore { get; set; }

    public string ReasonCode { get; set; } = string.Empty;
    public string ReasonDescription { get; set; } = string.Empty;
}
