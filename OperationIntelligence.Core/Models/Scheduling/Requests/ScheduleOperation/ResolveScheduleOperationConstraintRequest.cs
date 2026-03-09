namespace OperationIntelligence.Core.Models.Scheduling.Requests.ScheduleOperation;

public class ResolveScheduleOperationConstraintRequest
{
    public bool IsSatisfied { get; set; }
    public DateTime? SatisfiedAtUtc { get; set; }
}
