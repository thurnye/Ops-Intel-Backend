namespace OperationIntelligence.DB;

public class ResourceCapacitySnapshot : AuditableEntity
{
    public Guid ResourceId { get; set; }
    public ResourceType ResourceType { get; set; }

    public DateTime SnapshotDateUtc { get; set; }

    public Guid? ShiftId { get; set; }
    public Shift? Shift { get; set; }

    public int TotalCapacityMinutes { get; set; }
    public int ReservedMinutes { get; set; }
    public int AvailableMinutes { get; set; }
    public int OvertimeMinutes { get; set; }

    public bool IsOverloaded { get; set; }
    public bool IsBottleneck { get; set; }
}
