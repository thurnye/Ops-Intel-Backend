namespace OperationIntelligence.DB;

public class ProductSupplier: AuditableEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = default!;

    public Guid SupplierId { get; set; }
    public Supplier Supplier { get; set; } = default!;

    public string? SupplierProductCode { get; set; }
    public decimal SupplierPrice { get; set; }
    public int LeadTimeInDays { get; set; }

    public bool IsPreferredSupplier { get; set; } = false;
}
