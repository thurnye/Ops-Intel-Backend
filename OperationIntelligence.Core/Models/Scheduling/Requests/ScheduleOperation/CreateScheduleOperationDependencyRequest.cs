namespace OperationIntelligence.Core.Models.Scheduling.Requests.ScheduleOperation;

public class CreateScheduleOperationDependencyRequest
{
    public Guid PredecessorOperationId { get; set; }
    public Guid SuccessorOperationId { get; set; }
    public int DependencyType { get; set; }
    public int LagMinutes { get; set; }
    public bool IsMandatory { get; set; } = true;
}
