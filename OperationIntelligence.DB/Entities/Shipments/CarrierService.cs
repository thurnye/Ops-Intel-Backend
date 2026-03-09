namespace OperationIntelligence.DB;

public class CarrierService : AuditableEntity
{
    public Guid CarrierId { get; set; }
    public Carrier Carrier { get; set; } = default!;

    public string ServiceCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public int EstimatedTransitDays { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();
    public ICollection<ReturnShipment> ReturnShipments { get; set; } = new List<ReturnShipment>();
}