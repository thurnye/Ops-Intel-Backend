namespace OperationIntelligence.Core.Models.Scheduling.Responses.ScheduleOperation;

public class ScheduleOperationConstraintResponse
{
    public Guid Id { get; set; }
    public Guid ScheduleOperationId { get; set; }
    public int ConstraintType { get; set; }
    public string ConstraintTypeName { get; set; } = string.Empty;
    public string ReferenceNo { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsSatisfied { get; set; }
    public DateTime? SatisfiedAtUtc { get; set; }
    public bool IsMandatory { get; set; }
}
