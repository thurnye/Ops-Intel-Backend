using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class ReturnShipmentFilterRequest
{
    public string? Search { get; set; }
    public ReturnShipmentStatus? Status { get; set; }

    public Guid? ShipmentId { get; set; }
    public Guid? OrderId { get; set; }
    public Guid? CarrierId { get; set; }

    public DateTime? RequestedFromUtc { get; set; }
    public DateTime? RequestedToUtc { get; set; }

    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 25;
}
