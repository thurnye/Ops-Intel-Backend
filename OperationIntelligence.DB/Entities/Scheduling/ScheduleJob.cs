namespace OperationIntelligence.DB;

public class ScheduleJob : AuditableEntity
{
    public Guid SchedulePlanId { get; set; }
    public SchedulePlan SchedulePlan { get; set; } = default!;

    public Guid ProductionOrderId { get; set; }
    public ProductionOrder ProductionOrder { get; set; } = default!;

    public Guid? OrderId { get; set; }
    public Order? Order { get; set; }

    public Guid? OrderItemId { get; set; }
    public OrderItem? OrderItem { get; set; }

    public Guid ProductId { get; set; }
    public Product Product { get; set; } = default!;

    public Guid WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = default!;

    public string JobNumber { get; set; } = string.Empty;
    public string JobName { get; set; } = string.Empty;
    public string? Notes { get; set; }

    public decimal PlannedQuantity { get; set; }
    public decimal CompletedQuantity { get; set; }
    public decimal ScrappedQuantity { get; set; }

    public DateTime? EarliestStartUtc { get; set; }
    public DateTime? LatestFinishUtc { get; set; }
    public DateTime? DueDateUtc { get; set; }

    public SchedulePriority Priority { get; set; } = SchedulePriority.Normal;
    public ScheduleJobStatus Status { get; set; } = ScheduleJobStatus.Unscheduled;

    public bool MaterialsReady { get; set; }
    public MaterialReadinessStatus MaterialReadinessStatus { get; set; } = MaterialReadinessStatus.NotChecked;
    public DateTime? MaterialsCheckedAtUtc { get; set; }

    public bool QualityHold { get; set; }
    public bool IsRushOrder { get; set; }

    public ICollection<ScheduleOperation> ScheduleOperations { get; set; } = new List<ScheduleOperation>();
    public ICollection<ScheduleMaterialCheck> MaterialChecks { get; set; } = new List<ScheduleMaterialCheck>();
    public ICollection<ScheduleException> ScheduleExceptions { get; set; } = new List<ScheduleException>();
    public ICollection<ScheduleRescheduleHistory> RescheduleHistories { get; set; } = new List<ScheduleRescheduleHistory>();
    public ICollection<ScheduleStatusHistory> StatusHistories { get; set; } = new List<ScheduleStatusHistory>();
}
