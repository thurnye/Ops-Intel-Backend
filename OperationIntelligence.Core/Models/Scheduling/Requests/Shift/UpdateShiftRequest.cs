namespace OperationIntelligence.Core.Models.Scheduling.Requests.Shift;

public class UpdateShiftRequest
{
    public string ShiftName { get; set; } = string.Empty;

    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }

    public bool CrossesMidnight { get; set; }
    public bool IsActive { get; set; }

    public int CapacityMinutes { get; set; }
    public int BreakMinutes { get; set; }
}
