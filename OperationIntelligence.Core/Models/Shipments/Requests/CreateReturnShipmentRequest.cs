namespace OperationIntelligence.Core;

public class CreateReturnShipmentRequest
{
    public string ReturnShipmentNumber { get; set; } = string.Empty;

    public Guid ShipmentId { get; set; }
    public Guid? OrderId { get; set; }

    public Guid OriginAddressId { get; set; }
    public Guid DestinationAddressId { get; set; }

    public Guid? CarrierId { get; set; }
    public Guid? CarrierServiceId { get; set; }

    public string? TrackingNumber { get; set; }
    public string ReasonCode { get; set; } = string.Empty;
    public string? ReasonDescription { get; set; }

    public DateTime RequestedAtUtc { get; set; }
    public string? Notes { get; set; }
}
