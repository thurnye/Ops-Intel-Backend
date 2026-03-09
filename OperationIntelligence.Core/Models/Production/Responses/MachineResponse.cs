using OperationIntelligence.DB;

namespace OperationIntelligence.Core.Models.Production.Responses;

public class MachineResponse
{
    public Guid Id { get; set; }
    public string MachineCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Guid WorkCenterId { get; set; }
    public string? WorkCenterName { get; set; }
    public string? Model { get; set; }
    public string? Manufacturer { get; set; }
    public string? SerialNumber { get; set; }
    public decimal HourlyRunningCost { get; set; }
    public MachineStatus Status { get; set; }
    public DateTime? LastMaintenanceDate { get; set; }
    public DateTime? NextMaintenanceDate { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public string? UpdatedBy { get; set; }
}
