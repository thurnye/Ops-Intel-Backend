namespace OperationIntelligence.Core.Models.Production.Requests;

public class CreateProductionOutputRequest
{
    public Guid ProductionOrderId { get; set; }
    public Guid ProductId { get; set; }
    public Guid WarehouseId { get; set; }
    public decimal QuantityProduced { get; set; }
    public Guid UnitOfMeasureId { get; set; }
    public string? BatchNumber { get; set; }
    public string? LotNumber { get; set; }
    public DateTime OutputDate { get; set; }
    public bool IsFinalOutput { get; set; } = true;
    public string? Notes { get; set; }
}
