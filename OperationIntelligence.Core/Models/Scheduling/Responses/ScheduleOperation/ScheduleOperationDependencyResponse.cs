namespace OperationIntelligence.Core.Models.Scheduling.Responses.ScheduleOperation;

public class ScheduleOperationDependencyResponse
{
    public Guid Id { get; set; }
    public Guid PredecessorOperationId { get; set; }
    public string PredecessorOperationName { get; set; } = string.Empty;
    public Guid SuccessorOperationId { get; set; }
    public string SuccessorOperationName { get; set; } = string.Empty;
    public int DependencyType { get; set; }
    public string DependencyTypeName { get; set; } = string.Empty;
    public int LagMinutes { get; set; }
    public bool IsMandatory { get; set; }
}
