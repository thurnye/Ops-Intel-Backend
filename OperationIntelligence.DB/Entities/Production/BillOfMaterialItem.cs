namespace OperationIntelligence.DB;

public class BillOfMaterialItem : AuditableEntity
{
    public Guid BillOfMaterialId { get; set; }
    public BillOfMaterial BillOfMaterial { get; set; } = default!;

    public Guid MaterialProductId { get; set; }
    public Product MaterialProduct { get; set; } = default!;

    public decimal QuantityRequired { get; set; }

    public Guid UnitOfMeasureId { get; set; }
    public UnitOfMeasure UnitOfMeasure { get; set; } = default!;

    public decimal ScrapFactorPercent { get; set; }
    public decimal YieldFactorPercent { get; set; } = 100m;

    public bool IsOptional { get; set; }
    public bool IsBackflush { get; set; }

    public int Sequence { get; set; }

    public string? Notes { get; set; }
}