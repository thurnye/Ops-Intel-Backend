namespace OperationIntelligence.Core.Models.Scheduling.Responses.Shift;

public class ShiftResponse
{
    public Guid Id { get; set; }
    public Guid WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public Guid? WorkCenterId { get; set; }
    public string? WorkCenterName { get; set; }
    public string ShiftCode { get; set; } = string.Empty;
    public string ShiftName { get; set; } = string.Empty;
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool CrossesMidnight { get; set; }
    public bool IsActive { get; set; }
    public int CapacityMinutes { get; set; }
    public int BreakMinutes { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
