namespace OperationIntelligence.DB;

public class ScheduleMaterialCheck : AuditableEntity
{
    public Guid ScheduleJobId { get; set; }
    public ScheduleJob ScheduleJob { get; set; } = default!;

    public Guid? ScheduleOperationId { get; set; }
    public ScheduleOperation? ScheduleOperation { get; set; }

    public Guid ProductionOrderId { get; set; }
    public ProductionOrder ProductionOrder { get; set; } = default!;

    public Guid? RoutingStepId { get; set; }
    public RoutingStep? RoutingStep { get; set; }

    public Guid MaterialProductId { get; set; }
    public Product MaterialProduct { get; set; } = default!;

    public Guid WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = default!;

    public decimal RequiredQuantity { get; set; }
    public decimal AvailableQuantity { get; set; }
    public decimal ReservedQuantity { get; set; }
    public decimal ShortageQuantity { get; set; }

    public MaterialReadinessStatus Status { get; set; } = MaterialReadinessStatus.NotChecked;

    public DateTime? ExpectedAvailabilityDateUtc { get; set; }
    public string? Notes { get; set; }

    public DateTime CheckedAtUtc { get; set; }
}
