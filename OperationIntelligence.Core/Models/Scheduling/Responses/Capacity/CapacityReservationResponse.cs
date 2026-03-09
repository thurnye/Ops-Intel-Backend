namespace OperationIntelligence.Core.Models.Scheduling.Responses.Capacity;

public class CapacityReservationResponse
{
    public Guid Id { get; set; }
    public Guid ScheduleOperationId { get; set; }
    public Guid ResourceId { get; set; }
    public int ResourceType { get; set; }
    public string ResourceTypeName { get; set; } = string.Empty;
    public Guid? ShiftId { get; set; }
    public string? ShiftName { get; set; }
    public DateTime ReservedStartUtc { get; set; }
    public DateTime ReservedEndUtc { get; set; }
    public int ReservedMinutes { get; set; }
    public int AvailableMinutesAtBooking { get; set; }
    public int Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public string ReservationReason { get; set; } = string.Empty;
}
