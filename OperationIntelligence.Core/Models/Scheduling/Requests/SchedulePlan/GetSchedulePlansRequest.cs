using OperationIntelligence.Core.Models.Scheduling.Shared;

namespace OperationIntelligence.Core.Models.Scheduling.Requests.SchedulePlan;

public class GetSchedulePlansRequest : SchedulingDateRangeFilterRequest
{
    public Guid? WarehouseId { get; set; }
    public int? Status { get; set; }
    public int? GenerationMode { get; set; }
    public int? SchedulingStrategy { get; set; }
    public bool? IsActive { get; set; }
}
