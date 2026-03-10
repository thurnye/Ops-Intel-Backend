using OperationIntelligence.DB;

namespace OperationIntelligence.Core.Models.Production.Requests;

public class ProductionOrderQueryRequest
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public ProductionOrderStatus? Status { get; set; }
    public ProductionPriority? Priority { get; set; }
    public Guid? WarehouseId { get; set; }
    public DateTime? PlannedStartDateFrom { get; set; }
    public DateTime? PlannedStartDateTo { get; set; }
}
