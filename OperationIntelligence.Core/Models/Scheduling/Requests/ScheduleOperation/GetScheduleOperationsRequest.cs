using OperationIntelligence.Core.Models.Scheduling.Shared;

namespace OperationIntelligence.Core.Models.Scheduling.Requests.ScheduleOperation;

public class GetScheduleOperationsRequest : SchedulingDateRangeFilterRequest
{
    public Guid? ScheduleJobId { get; set; }
    public Guid? RoutingStepId { get; set; }
    public Guid? WorkCenterId { get; set; }
    public Guid? MachineId { get; set; }
    public Guid? PlannedShiftId { get; set; }
    public Guid? ActualShiftId { get; set; }

    public int? Status { get; set; }
    public int? DispatchStatus { get; set; }

    public bool? IsCriticalPath { get; set; }
    public bool? IsBottleneckOperation { get; set; }
    public bool? IsOutsourced { get; set; }
}
