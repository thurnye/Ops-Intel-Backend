using OperationIntelligence.Core.Models.Scheduling.Responses.Capacity;
using OperationIntelligence.Core.Models.Scheduling.Responses.Dispatch;

namespace OperationIntelligence.Core.Models.Scheduling.Responses.ScheduleOperation;

public class ScheduleOperationDetailResponse : ScheduleOperationResponse
{
    public List<ScheduleOperationDependencyResponse> Dependencies { get; set; } = new();
    public List<ScheduleOperationConstraintResponse> Constraints { get; set; } = new();
    public List<ScheduleOperationResourceOptionResponse> ResourceOptions { get; set; } = new();
    public List<ScheduleResourceAssignmentResponse> ResourceAssignments { get; set; } = new();
    public List<CapacityReservationResponse> CapacityReservations { get; set; } = new();
    public List<DispatchQueueItemResponse> DispatchQueueItems { get; set; } = new();
}
