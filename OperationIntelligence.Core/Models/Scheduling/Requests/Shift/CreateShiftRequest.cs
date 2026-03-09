namespace OperationIntelligence.Core.Models.Scheduling.Requests.Shift;

public class CreateShiftRequest
{
    public Guid WarehouseId { get; set; }
    public Guid? WorkCenterId { get; set; }

    public string ShiftCode { get; set; } = string.Empty;
    public string ShiftName { get; set; } = string.Empty;

    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }

    public bool CrossesMidnight { get; set; }
    public int CapacityMinutes { get; set; }
    public int BreakMinutes { get; set; }
}
