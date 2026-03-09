namespace OperationIntelligence.Core.Models.Scheduling.Requests.Capacity;

public class UpdateCapacityReservationRequest
{
    public Guid? ShiftId { get; set; }

    public DateTime ReservedStartUtc { get; set; }
    public DateTime ReservedEndUtc { get; set; }

    public int ReservedMinutes { get; set; }
    public int AvailableMinutesAtBooking { get; set; }

    public int Status { get; set; }
    public string ReservationReason { get; set; } = string.Empty;
}
