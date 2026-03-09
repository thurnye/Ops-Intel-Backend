namespace OperationIntelligence.Core.Models.Scheduling.Responses.ScheduleOperation;

public class ScheduleOperationResourceOptionResponse
{
    public Guid Id { get; set; }
    public Guid ScheduleOperationId { get; set; }
    public Guid ResourceId { get; set; }
    public int ResourceType { get; set; }
    public string ResourceTypeName { get; set; } = string.Empty;
    public bool IsPrimaryOption { get; set; }
    public int PreferenceRank { get; set; }
    public decimal EfficiencyFactor { get; set; }
    public decimal SetupPenaltyMinutes { get; set; }
    public bool IsActive { get; set; }
}
