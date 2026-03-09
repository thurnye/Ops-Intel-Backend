using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class DockAppointmentResponse
{
    public Guid Id { get; set; }
    public string AppointmentNumber { get; set; } = string.Empty;
    public Guid WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public Guid? CarrierId { get; set; }
    public string? CarrierName { get; set; }
    public string? DockCode { get; set; }
    public string? TrailerNumber { get; set; }
    public string? DriverName { get; set; }
    public DateTime ScheduledStartUtc { get; set; }
    public DateTime ScheduledEndUtc { get; set; }
    public DateTime? ActualArrivalUtc { get; set; }
    public DateTime? ActualDepartureUtc { get; set; }
    public DockAppointmentStatus Status { get; set; }
    public string? Notes { get; set; }
}
