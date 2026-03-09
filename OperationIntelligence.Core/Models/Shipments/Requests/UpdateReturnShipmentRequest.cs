using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class UpdateReturnShipmentRequest
{
    public Guid OriginAddressId { get; set; }
    public Guid DestinationAddressId { get; set; }

    public Guid? CarrierId { get; set; }
    public Guid? CarrierServiceId { get; set; }

    public string? TrackingNumber { get; set; }
    public string ReasonCode { get; set; } = string.Empty;
    public string? ReasonDescription { get; set; }

    public ReturnShipmentStatus Status { get; set; }
    public DateTime? ReceivedAtUtc { get; set; }

    public string? Notes { get; set; }
}
