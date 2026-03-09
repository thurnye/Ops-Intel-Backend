namespace OperationIntelligence.Core;

public class CreateDeliveryRunRequest
{
    public string RunNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Guid WarehouseId { get; set; }

    public DateTime PlannedStartUtc { get; set; }
    public DateTime PlannedEndUtc { get; set; }

    public string? DriverName { get; set; }
    public string? VehicleNumber { get; set; }
    public string? RouteCode { get; set; }
    public string? Notes { get; set; }
}
