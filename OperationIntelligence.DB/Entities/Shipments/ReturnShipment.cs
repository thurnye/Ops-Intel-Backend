namespace OperationIntelligence.DB;

public class ReturnShipment : AuditableEntity
{
    public string ReturnShipmentNumber { get; set; } = string.Empty;

    public Guid ShipmentId { get; set; }
    public Shipment Shipment { get; set; } = default!;

    public Guid? OrderId { get; set; }
    public Order? Order { get; set; }

    public Guid OriginAddressId { get; set; }
    public ShipmentAddress OriginAddress { get; set; } = default!;

    public Guid DestinationAddressId { get; set; }
    public ShipmentAddress DestinationAddress { get; set; } = default!;

    public Guid? CarrierId { get; set; }
    public Carrier? Carrier { get; set; }

    public Guid? CarrierServiceId { get; set; }
    public CarrierService? CarrierService { get; set; }

    public string? TrackingNumber { get; set; }
    public string ReasonCode { get; set; } = string.Empty;
    public string? ReasonDescription { get; set; }

    public ReturnShipmentStatus Status { get; set; } = ReturnShipmentStatus.Requested;

    public DateTime RequestedAtUtc { get; set; }
    public DateTime? ReceivedAtUtc { get; set; }

    public string? Notes { get; set; }

    public ICollection<ReturnShipmentItem> Items { get; set; } = new List<ReturnShipmentItem>();
}