namespace OperationIntelligence.Core.Models.Production.Requests;

public class CreateBillOfMaterialItemRequest
{
    public Guid BillOfMaterialId { get; set; }
    public Guid MaterialProductId { get; set; }
    public decimal QuantityRequired { get; set; }
    public Guid UnitOfMeasureId { get; set; }
    public decimal ScrapFactorPercent { get; set; }
    public decimal YieldFactorPercent { get; set; } = 100m;
    public bool IsOptional { get; set; }
    public bool IsBackflush { get; set; }
    public int Sequence { get; set; }
    public string? Notes { get; set; }
}
