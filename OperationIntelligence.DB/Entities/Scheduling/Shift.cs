namespace OperationIntelligence.DB;

public class Shift : AuditableEntity
{
    public Guid WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = default!;

    public Guid? WorkCenterId { get; set; }
    public WorkCenter? WorkCenter { get; set; }

    public string ShiftCode { get; set; } = string.Empty;
    public string ShiftName { get; set; } = string.Empty;

    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }

    public bool CrossesMidnight { get; set; }
    public bool IsActive { get; set; } = true;

    public int CapacityMinutes { get; set; }
    public int BreakMinutes { get; set; }

    public ICollection<ScheduleOperation> PlannedOperations { get; set; } = new List<ScheduleOperation>();
    public ICollection<ScheduleOperation> ActualOperations { get; set; } = new List<ScheduleOperation>();
    public ICollection<ScheduleResourceAssignment> ResourceAssignments { get; set; } = new List<ScheduleResourceAssignment>();
    public ICollection<CapacityReservation> CapacityReservations { get; set; } = new List<CapacityReservation>();
    public ICollection<ResourceCapacitySnapshot> CapacitySnapshots { get; set; } = new List<ResourceCapacitySnapshot>();
}
