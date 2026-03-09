using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class UpdateShipmentStatusRequest
{
    public ShipmentStatus Status { get; set; }
    public string? Reason { get; set; }
}
