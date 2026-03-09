namespace OperationIntelligence.Core.Models.Production.Responses;

public class BillOfMaterialResponse
{
    public Guid Id { get; set; }
    public string BomCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Guid ProductId { get; set; }
    public string? ProductName { get; set; }
    public string? ProductSku { get; set; }
    public decimal BaseQuantity { get; set; }
    public Guid UnitOfMeasureId { get; set; }
    public string? UnitOfMeasureName { get; set; }
    public int Version { get; set; }
    public bool IsActive { get; set; }
    public bool IsDefault { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public string? Notes { get; set; }
    public List<BillOfMaterialItemResponse> Items { get; set; } = new();
    public DateTime CreatedAtUtc { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public string? UpdatedBy { get; set; }
}
