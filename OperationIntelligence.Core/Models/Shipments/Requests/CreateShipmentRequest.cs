using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class CreateShipmentRequest
{
    public Guid? OrderId { get; set; }
    public Guid WarehouseId { get; set; }

    public Guid OriginAddressId { get; set; }
    public Guid DestinationAddressId { get; set; }

    public Guid? CarrierId { get; set; }
    public Guid? CarrierServiceId { get; set; }

    public ShipmentType Type { get; set; } = ShipmentType.Outbound;
    public ShipmentPriority Priority { get; set; } = ShipmentPriority.Normal;

    public string? CustomerReference { get; set; }
    public string? ExternalReference { get; set; }
    public string? TrackingNumber { get; set; }
    public string? MasterTrackingNumber { get; set; }

    public DateTime? PlannedShipDateUtc { get; set; }
    public DateTime? PlannedDeliveryDateUtc { get; set; }
    public DateTime? ScheduledPickupStartUtc { get; set; }
    public DateTime? ScheduledPickupEndUtc { get; set; }

    public bool IsPartialShipment { get; set; }
    public bool RequiresSignature { get; set; }
    public bool IsFragile { get; set; }
    public bool IsHazardous { get; set; }
    public bool IsTemperatureControlled { get; set; }
    public bool IsInsured { get; set; }
    public bool IsCrossBorder { get; set; }

    public string CurrencyCode { get; set; } = "CAD";
    public string? ShippingTerms { get; set; }
    public string? Incoterm { get; set; }

    public string? Notes { get; set; }
    public string? InternalNotes { get; set; }
}
