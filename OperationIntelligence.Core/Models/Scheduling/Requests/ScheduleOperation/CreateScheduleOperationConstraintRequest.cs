namespace OperationIntelligence.Core.Models.Scheduling.Requests.ScheduleOperation;

public class CreateScheduleOperationConstraintRequest
{
    public Guid ScheduleOperationId { get; set; }
    public int ConstraintType { get; set; }
    public string ReferenceNo { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsMandatory { get; set; } = true;
}
