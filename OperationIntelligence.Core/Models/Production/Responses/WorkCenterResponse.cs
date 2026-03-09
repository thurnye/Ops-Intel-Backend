namespace OperationIntelligence.Core.Models.Production.Responses;

public class WorkCenterResponse
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid WarehouseId { get; set; }
    public string? WarehouseName { get; set; }
    public decimal CapacityPerDay { get; set; }
    public int AvailableOperators { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public string? UpdatedBy { get; set; }
}
