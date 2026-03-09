using OperationIntelligence.Core.Models.Scheduling.Shared;

namespace OperationIntelligence.Core.Models.Scheduling.Requests.Exception;

public class GetScheduleExceptionsRequest : SchedulingDateRangeFilterRequest
{
    public Guid? SchedulePlanId { get; set; }
    public Guid? ScheduleJobId { get; set; }
    public Guid? ScheduleOperationId { get; set; }

    public int? ExceptionType { get; set; }
    public int? Severity { get; set; }
    public int? Status { get; set; }

    public string? AssignedTo { get; set; }
}
