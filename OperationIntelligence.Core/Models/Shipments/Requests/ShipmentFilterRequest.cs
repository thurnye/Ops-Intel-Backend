using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class ShipmentFilterRequest
{
    public string? Search { get; set; }
    public ShipmentStatus? Status { get; set; }
    public ShipmentType? Type { get; set; }
    public ShipmentPriority? Priority { get; set; }

    public Guid? OrderId { get; set; }
    public Guid? WarehouseId { get; set; }
    public Guid? CarrierId { get; set; }

    public DateTime? PlannedShipDateFromUtc { get; set; }
    public DateTime? PlannedShipDateToUtc { get; set; }

    public bool? IsCrossBorder { get; set; }
    public bool? IsPartialShipment { get; set; }

    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 25;
}
