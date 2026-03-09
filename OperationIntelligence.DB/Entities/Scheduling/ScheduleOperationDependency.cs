namespace OperationIntelligence.DB;

public class ScheduleOperationDependency : AuditableEntity
{
    public Guid PredecessorOperationId { get; set; }
    public ScheduleOperation PredecessorOperation { get; set; } = default!;

    public Guid SuccessorOperationId { get; set; }
    public ScheduleOperation SuccessorOperation { get; set; } = default!;

    public DependencyType DependencyType { get; set; } = DependencyType.FinishToStart;
    public int LagMinutes { get; set; }
    public bool IsMandatory { get; set; } = true;
}
