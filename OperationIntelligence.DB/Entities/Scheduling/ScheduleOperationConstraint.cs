namespace OperationIntelligence.DB;

public class ScheduleOperationConstraint : AuditableEntity
{
    public Guid ScheduleOperationId { get; set; }
    public ScheduleOperation ScheduleOperation { get; set; } = default!;

    public OperationConstraintType ConstraintType { get; set; }
    public string ReferenceNo { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public bool IsSatisfied { get; set; }
    public DateTime? SatisfiedAtUtc { get; set; }
    public bool IsMandatory { get; set; } = true;
}
