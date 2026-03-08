namespace OperationIntelligence.Core;

public class ProductSupplierResponse
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;

    public Guid SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;

    public string? SupplierProductCode { get; set; }
    public decimal SupplierPrice { get; set; }
    public int LeadTimeInDays { get; set; }
    public bool IsPreferredSupplier { get; set; }
}
