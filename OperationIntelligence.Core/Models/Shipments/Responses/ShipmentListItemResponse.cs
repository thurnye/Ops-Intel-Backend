using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class ShipmentListItemResponse
{
    public Guid Id { get; set; }
    public string ShipmentNumber { get; set; } = string.Empty;
    public Guid? OrderId { get; set; }
    public string? OrderNumber { get; set; }
    public string? WarehouseName { get; set; }
    public string? CarrierName { get; set; }
    public ShipmentType Type { get; set; }
    public ShipmentStatus Status { get; set; }
    public ShipmentPriority Priority { get; set; }
    public string? TrackingNumber { get; set; }
    public DateTime? PlannedShipDateUtc { get; set; }
    public DateTime? PlannedDeliveryDateUtc { get; set; }
    public decimal TotalWeight { get; set; }
    public int TotalPackages { get; set; }
    public bool IsCrossBorder { get; set; }
    public bool IsPartialShipment { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
