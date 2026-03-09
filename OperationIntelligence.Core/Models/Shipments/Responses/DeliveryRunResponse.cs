using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class DeliveryRunResponse
{
    public Guid Id { get; set; }
    public string RunNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Guid WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public DateTime PlannedStartUtc { get; set; }
    public DateTime PlannedEndUtc { get; set; }
    public DateTime? ActualStartUtc { get; set; }
    public DateTime? ActualEndUtc { get; set; }
    public string? DriverName { get; set; }
    public string? VehicleNumber { get; set; }
    public string? RouteCode { get; set; }
    public DeliveryRunStatus Status { get; set; }
    public string? Notes { get; set; }
}
