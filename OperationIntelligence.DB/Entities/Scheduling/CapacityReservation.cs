namespace OperationIntelligence.DB;

public class CapacityReservation : AuditableEntity
{
    public Guid ScheduleOperationId { get; set; }
    public ScheduleOperation ScheduleOperation { get; set; } = default!;

    public Guid ResourceId { get; set; }
    public ResourceType ResourceType { get; set; }

    public Guid? ShiftId { get; set; }
    public Shift? Shift { get; set; }

    public DateTime ReservedStartUtc { get; set; }
    public DateTime ReservedEndUtc { get; set; }

    public int ReservedMinutes { get; set; }
    public int AvailableMinutesAtBooking { get; set; }

    public CapacityReservationStatus Status { get; set; } = CapacityReservationStatus.Reserved;
    public string ReservationReason { get; set; } = string.Empty;
}
