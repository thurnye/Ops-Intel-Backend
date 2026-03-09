namespace OperationIntelligence.DB;

public class DeliveryRun : AuditableEntity
{
    public string RunNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public Guid WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = default!;

    public DateTime PlannedStartUtc { get; set; }
    public DateTime PlannedEndUtc { get; set; }
    public DateTime? ActualStartUtc { get; set; }
    public DateTime? ActualEndUtc { get; set; }

    public string? DriverName { get; set; }
    public string? VehicleNumber { get; set; }
    public string? RouteCode { get; set; }

    public DeliveryRunStatus Status { get; set; } = DeliveryRunStatus.Planned;
    public string? Notes { get; set; }

    public ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();
}