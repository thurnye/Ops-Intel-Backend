using OperationIntelligence.Core.Models.Scheduling.Shared;

namespace OperationIntelligence.Core.Models.Scheduling.Requests.Shift;

public class GetShiftsRequest : SchedulingPagedFilterRequest
{
    public Guid? WarehouseId { get; set; }
    public Guid? WorkCenterId { get; set; }
    public bool? IsActive { get; set; }
}
