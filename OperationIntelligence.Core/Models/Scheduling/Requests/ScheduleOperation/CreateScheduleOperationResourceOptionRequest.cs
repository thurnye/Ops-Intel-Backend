namespace OperationIntelligence.Core.Models.Scheduling.Requests.ScheduleOperation;

public class CreateScheduleOperationResourceOptionRequest
{
    public Guid ScheduleOperationId { get; set; }
    public Guid ResourceId { get; set; }
    public int ResourceType { get; set; }
    public bool IsPrimaryOption { get; set; }
    public int PreferenceRank { get; set; }
    public decimal EfficiencyFactor { get; set; } = 1m;
    public decimal SetupPenaltyMinutes { get; set; }
    public bool IsActive { get; set; } = true;
}
