namespace OperationIntelligence.Core.Models.Scheduling.Requests.Capacity;

public class CreateCapacityReservationRequest
{
    public Guid ScheduleOperationId { get; set; }
    public Guid ResourceId { get; set; }
    public int ResourceType { get; set; }
    public Guid? ShiftId { get; set; }

    public DateTime ReservedStartUtc { get; set; }
    public DateTime ReservedEndUtc { get; set; }

    public int ReservedMinutes { get; set; }
    public int AvailableMinutesAtBooking { get; set; }

    public string ReservationReason { get; set; } = string.Empty;
}
