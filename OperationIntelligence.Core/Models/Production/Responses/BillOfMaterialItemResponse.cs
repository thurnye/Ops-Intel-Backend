namespace OperationIntelligence.Core.Models.Production.Responses;

public class BillOfMaterialItemResponse
{
    public Guid Id { get; set; }
    public Guid BillOfMaterialId { get; set; }
    public Guid MaterialProductId { get; set; }
    public string? MaterialProductName { get; set; }
    public string? MaterialProductSku { get; set; }
    public decimal QuantityRequired { get; set; }
    public Guid UnitOfMeasureId { get; set; }
    public string? UnitOfMeasureName { get; set; }
    public decimal ScrapFactorPercent { get; set; }
    public decimal YieldFactorPercent { get; set; }
    public bool IsOptional { get; set; }
    public bool IsBackflush { get; set; }
    public int Sequence { get; set; }
    public string? Notes { get; set; }
}
