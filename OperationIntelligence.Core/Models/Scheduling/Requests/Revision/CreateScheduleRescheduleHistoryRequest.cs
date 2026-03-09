namespace OperationIntelligence.Core.Models.Scheduling.Requests.Revision;

public class CreateScheduleRescheduleHistoryRequest
{
    public Guid? SchedulePlanId { get; set; }
    public Guid? ScheduleJobId { get; set; }
    public Guid? ScheduleOperationId { get; set; }

    public DateTime? OldPlannedStartUtc { get; set; }
    public DateTime? OldPlannedEndUtc { get; set; }
    public DateTime? NewPlannedStartUtc { get; set; }
    public DateTime? NewPlannedEndUtc { get; set; }

    public Guid? OldWorkCenterId { get; set; }
    public Guid? NewWorkCenterId { get; set; }
    public Guid? OldMachineId { get; set; }
    public Guid? NewMachineId { get; set; }

    public string ReasonCode { get; set; } = string.Empty;
    public string ReasonDescription { get; set; } = string.Empty;

    public DateTime ChangedAtUtc { get; set; }
}
