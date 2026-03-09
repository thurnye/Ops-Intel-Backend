namespace OperationIntelligence.DB;

public class Carrier : AuditableEntity
{
    public string CarrierCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? ContactName { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<CarrierService> Services { get; set; } = new List<CarrierService>();
    public ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();
    public ICollection<DockAppointment> DockAppointments { get; set; } = new List<DockAppointment>();
    public ICollection<ReturnShipment> ReturnShipments { get; set; } = new List<ReturnShipment>();
}