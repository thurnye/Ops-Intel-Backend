namespace OperationIntelligence.Core.Models.Scheduling.Responses.Capacity;

public class ResourceCapacitySnapshotResponse
{
    public Guid Id { get; set; }
    public Guid ResourceId { get; set; }
    public int ResourceType { get; set; }
    public string ResourceTypeName { get; set; } = string.Empty;
    public DateTime SnapshotDateUtc { get; set; }
    public Guid? ShiftId { get; set; }
    public string? ShiftName { get; set; }
    public int TotalCapacityMinutes { get; set; }
    public int ReservedMinutes { get; set; }
    public int AvailableMinutes { get; set; }
    public int OvertimeMinutes { get; set; }
    public bool IsOverloaded { get; set; }
    public bool IsBottleneck { get; set; }
}
