namespace OperationIntelligence.DB;

public class ShipmentStatusHistory : AuditableEntity
{
    public Guid ShipmentId { get; set; }
    public Shipment Shipment { get; set; } = default!;

    public ShipmentStatus FromStatus { get; set; }
    public ShipmentStatus ToStatus { get; set; }

    public DateTime ChangedAtUtc { get; set; }
    public string ChangedBy { get; set; } = string.Empty;
    public string? Reason { get; set; }
}