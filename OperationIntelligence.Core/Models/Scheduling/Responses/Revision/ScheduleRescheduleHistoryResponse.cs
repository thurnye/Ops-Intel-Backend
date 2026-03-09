namespace OperationIntelligence.Core.Models.Scheduling.Responses.Revision;

public class ScheduleRescheduleHistoryResponse
{
    public Guid Id { get; set; }
    public Guid? SchedulePlanId { get; set; }
    public Guid? ScheduleJobId { get; set; }
    public Guid? ScheduleOperationId { get; set; }

    public DateTime? OldPlannedStartUtc { get; set; }
    public DateTime? OldPlannedEndUtc { get; set; }
    public DateTime? NewPlannedStartUtc { get; set; }
    public DateTime? NewPlannedEndUtc { get; set; }

    public Guid? OldWorkCenterId { get; set; }
    public string? OldWorkCenterName { get; set; }
    public Guid? NewWorkCenterId { get; set; }
    public string? NewWorkCenterName { get; set; }

    public Guid? OldMachineId { get; set; }
    public string? OldMachineName { get; set; }
    public Guid? NewMachineId { get; set; }
    public string? NewMachineName { get; set; }

    public string ReasonCode { get; set; } = string.Empty;
    public string ReasonDescription { get; set; } = string.Empty;

    public DateTime ChangedAtUtc { get; set; }
}
