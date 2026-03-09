using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class ReturnShipmentResponse
{
    public Guid Id { get; set; }
    public string ReturnShipmentNumber { get; set; } = string.Empty;
    public Guid ShipmentId { get; set; }
    public Guid? OrderId { get; set; }

    public Guid OriginAddressId { get; set; }
    public Guid DestinationAddressId { get; set; }

    public Guid? CarrierId { get; set; }
    public string? CarrierName { get; set; }

    public Guid? CarrierServiceId { get; set; }
    public string? CarrierServiceName { get; set; }

    public string? TrackingNumber { get; set; }
    public string ReasonCode { get; set; } = string.Empty;
    public string? ReasonDescription { get; set; }

    public ReturnShipmentStatus Status { get; set; }
    public DateTime RequestedAtUtc { get; set; }
    public DateTime? ReceivedAtUtc { get; set; }
    public string? Notes { get; set; }

    public List<ReturnShipmentItemResponse> Items { get; set; } = new();
}
