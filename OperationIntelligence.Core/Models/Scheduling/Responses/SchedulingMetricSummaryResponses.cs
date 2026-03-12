namespace OperationIntelligence.Core;

public class ShiftMetricsSummaryResponse
{
    public int TotalShifts { get; set; }
    public int ActiveShifts { get; set; }
    public int OvernightShifts { get; set; }
    public int WorkCentersRepresented { get; set; }
}

public class DispatchMetricsSummaryResponse
{
    public int TotalJobs { get; set; }
    public int ReleasedJobs { get; set; }
    public int RunningJobs { get; set; }
    public int OpenBlockers { get; set; }
}
