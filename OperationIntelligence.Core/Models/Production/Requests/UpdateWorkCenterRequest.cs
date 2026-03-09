namespace OperationIntelligence.Core.Models.Production.Requests;

public class UpdateWorkCenterRequest
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid WarehouseId { get; set; }
    public decimal CapacityPerDay { get; set; }
    public int AvailableOperators { get; set; }
    public bool IsActive { get; set; }
}
