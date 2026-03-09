using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class ShipmentPackageResponse
{
    public Guid Id { get; set; }
    public string PackageNumber { get; set; } = string.Empty;
    public string? TrackingNumber { get; set; }
    public string PackageType { get; set; } = string.Empty;
    public decimal Length { get; set; }
    public decimal Width { get; set; }
    public decimal Height { get; set; }
    public decimal Weight { get; set; }
    public decimal DeclaredValue { get; set; }
    public bool RequiresSpecialHandling { get; set; }
    public bool IsFragile { get; set; }
    public string? LabelUrl { get; set; }
    public string? Barcode { get; set; }
    public PackageStatus Status { get; set; }
    public List<ShipmentPackageItemResponse> PackageItems { get; set; } = new();
}
