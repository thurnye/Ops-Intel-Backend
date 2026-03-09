namespace OperationIntelligence.DB;

public class BillOfMaterial : AuditableEntity
{
    public string BomCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public Guid ProductId { get; set; }
    public Product Product { get; set; } = default!;

    public decimal BaseQuantity { get; set; }

    public Guid UnitOfMeasureId { get; set; }
    public UnitOfMeasure UnitOfMeasure { get; set; } = default!;

    public int Version { get; set; } = 1;

    public bool IsActive { get; set; } = true;
    public bool IsDefault { get; set; }

    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }

    public string? Notes { get; set; }

    public ICollection<BillOfMaterialItem> Items { get; set; } = new List<BillOfMaterialItem>();
    public ICollection<ProductionOrder> ProductionOrders { get; set; } = new List<ProductionOrder>();
}