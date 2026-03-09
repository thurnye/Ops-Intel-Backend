namespace OperationIntelligence.Core.Models.Scheduling.Requests.Material;

public class CreateScheduleMaterialCheckRequest
{
    public Guid ScheduleJobId { get; set; }
    public Guid? ScheduleOperationId { get; set; }

    public Guid ProductionOrderId { get; set; }
    public Guid? RoutingStepId { get; set; }

    public Guid MaterialProductId { get; set; }
    public Guid WarehouseId { get; set; }

    public decimal RequiredQuantity { get; set; }
    public decimal AvailableQuantity { get; set; }
    public decimal ReservedQuantity { get; set; }
    public decimal ShortageQuantity { get; set; }

    public int Status { get; set; }

    public DateTime? ExpectedAvailabilityDateUtc { get; set; }
    public string? Notes { get; set; }
    public DateTime CheckedAtUtc { get; set; }
}
