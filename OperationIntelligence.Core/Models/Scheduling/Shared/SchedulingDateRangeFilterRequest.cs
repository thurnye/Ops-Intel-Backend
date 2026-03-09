namespace OperationIntelligence.Core.Models.Scheduling.Shared;

public class SchedulingDateRangeFilterRequest : SchedulingPagedFilterRequest
{
    public DateTime? StartDateUtc { get; set; }
    public DateTime? EndDateUtc { get; set; }
}
