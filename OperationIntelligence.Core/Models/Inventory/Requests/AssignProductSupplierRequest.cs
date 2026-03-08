namespace OperationIntelligence.Core;

public class AssignProductSupplierRequest
{
    public Guid ProductId { get; set; }
    public Guid SupplierId { get; set; }
    public string? SupplierProductCode { get; set; }
    public decimal SupplierPrice { get; set; }
    public int LeadTimeInDays { get; set; }
    public bool IsPreferredSupplier { get; set; }
}
