namespace OperationIntelligence.Core.Models.Scheduling.Responses.Material;

public class ScheduleMaterialCheckResponse
{
    public Guid Id { get; set; }
    public Guid ScheduleJobId { get; set; }
    public Guid? ScheduleOperationId { get; set; }
    public Guid ProductionOrderId { get; set; }
    public Guid? RoutingStepId { get; set; }
    public Guid MaterialProductId { get; set; }
    public string MaterialProductName { get; set; } = string.Empty;
    public Guid WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public decimal RequiredQuantity { get; set; }
    public decimal AvailableQuantity { get; set; }
    public decimal ReservedQuantity { get; set; }
    public decimal ShortageQuantity { get; set; }
    public int Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public DateTime? ExpectedAvailabilityDateUtc { get; set; }
    public string? Notes { get; set; }
    public DateTime CheckedAtUtc { get; set; }
}
