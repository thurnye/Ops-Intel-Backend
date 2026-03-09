namespace OperationIntelligence.Core.Models.Scheduling.Requests.ScheduleJob;

public class CreateScheduleJobRequest
{
    public Guid SchedulePlanId { get; set; }

    public Guid ProductionOrderId { get; set; }
    public Guid? OrderId { get; set; }
    public Guid? OrderItemId { get; set; }

    public Guid ProductId { get; set; }
    public Guid WarehouseId { get; set; }

    public string JobNumber { get; set; } = string.Empty;
    public string JobName { get; set; } = string.Empty;
    public string? Notes { get; set; }

    public decimal PlannedQuantity { get; set; }

    public DateTime? EarliestStartUtc { get; set; }
    public DateTime? LatestFinishUtc { get; set; }
    public DateTime? DueDateUtc { get; set; }

    public int Priority { get; set; }
    public bool IsRushOrder { get; set; }
    public bool QualityHold { get; set; }
}
