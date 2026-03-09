namespace OperationIntelligence.Core;

public class CreateDockAppointmentRequest
{
    public string AppointmentNumber { get; set; } = string.Empty;
    public Guid WarehouseId { get; set; }
    public Guid? CarrierId { get; set; }

    public string? DockCode { get; set; }
    public string? TrailerNumber { get; set; }
    public string? DriverName { get; set; }

    public DateTime ScheduledStartUtc { get; set; }
    public DateTime ScheduledEndUtc { get; set; }

    public string? Notes { get; set; }
}
