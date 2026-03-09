namespace OperationIntelligence.Core.Models.Scheduling.Requests.SchedulePlan;

public class CloneSchedulePlanRequest
{
    public string NewPlanNumber { get; set; } = string.Empty;
    public string NewName { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public bool CopyJobs { get; set; } = true;
    public bool CopyOperations { get; set; } = true;
    public bool CopyAssignments { get; set; } = true;
}
