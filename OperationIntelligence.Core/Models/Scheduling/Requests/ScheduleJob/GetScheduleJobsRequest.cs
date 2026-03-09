using OperationIntelligence.Core.Models.Scheduling.Shared;

namespace OperationIntelligence.Core.Models.Scheduling.Requests.ScheduleJob;

public class GetScheduleJobsRequest : SchedulingDateRangeFilterRequest
{
    public Guid? SchedulePlanId { get; set; }
    public Guid? ProductionOrderId { get; set; }
    public Guid? OrderId { get; set; }
    public Guid? ProductId { get; set; }
    public Guid? WarehouseId { get; set; }

    public int? Status { get; set; }
    public int? Priority { get; set; }

    public bool? MaterialsReady { get; set; }
    public int? MaterialReadinessStatus { get; set; }
    public bool? QualityHold { get; set; }
    public bool? IsRushOrder { get; set; }
}
