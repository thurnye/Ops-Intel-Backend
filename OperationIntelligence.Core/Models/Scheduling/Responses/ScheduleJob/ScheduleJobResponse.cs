namespace OperationIntelligence.Core.Models.Scheduling.Responses.ScheduleJob;

public class ScheduleJobResponse
{
    public Guid Id { get; set; }
    public Guid SchedulePlanId { get; set; }

    public Guid ProductionOrderId { get; set; }
    public string ProductionOrderNumber { get; set; } = string.Empty;

    public Guid? OrderId { get; set; }
    public string? OrderNumber { get; set; }

    public Guid? OrderItemId { get; set; }

    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;

    public Guid WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;

    public string JobNumber { get; set; } = string.Empty;
    public string JobName { get; set; } = string.Empty;
    public string? Notes { get; set; }

    public decimal PlannedQuantity { get; set; }
    public decimal CompletedQuantity { get; set; }
    public decimal ScrappedQuantity { get; set; }

    public DateTime? EarliestStartUtc { get; set; }
    public DateTime? LatestFinishUtc { get; set; }
    public DateTime? DueDateUtc { get; set; }

    public int Priority { get; set; }
    public string PriorityName { get; set; } = string.Empty;

    public int Status { get; set; }
    public string StatusName { get; set; } = string.Empty;

    public bool MaterialsReady { get; set; }
    public int MaterialReadinessStatus { get; set; }
    public string MaterialReadinessStatusName { get; set; } = string.Empty;
    public DateTime? MaterialsCheckedAtUtc { get; set; }

    public bool QualityHold { get; set; }
    public bool IsRushOrder { get; set; }

    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
