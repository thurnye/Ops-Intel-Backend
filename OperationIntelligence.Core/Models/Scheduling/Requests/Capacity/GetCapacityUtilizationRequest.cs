using OperationIntelligence.Core.Models.Scheduling.Shared;

namespace OperationIntelligence.Core.Models.Scheduling.Requests.Capacity;

public class GetCapacityUtilizationRequest : SchedulingDateRangeFilterRequest
{
    public Guid? ResourceId { get; set; }
    public int? ResourceType { get; set; }
    public Guid? ShiftId { get; set; }
    public bool? IsOverloaded { get; set; }
    public bool? IsBottleneck { get; set; }
}
