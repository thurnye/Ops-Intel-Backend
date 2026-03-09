namespace OperationIntelligence.DB;

public class DockAppointment : AuditableEntity
{
    public string AppointmentNumber { get; set; } = string.Empty;

    public Guid WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = default!;

    public Guid? CarrierId { get; set; }
    public Carrier? Carrier { get; set; }

    public string? DockCode { get; set; }
    public string? TrailerNumber { get; set; }
    public string? DriverName { get; set; }

    public DateTime ScheduledStartUtc { get; set; }
    public DateTime ScheduledEndUtc { get; set; }
    public DateTime? ActualArrivalUtc { get; set; }
    public DateTime? ActualDepartureUtc { get; set; }

    public DockAppointmentStatus Status { get; set; } = DockAppointmentStatus.Scheduled;
    public string? Notes { get; set; }

    public ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();
}