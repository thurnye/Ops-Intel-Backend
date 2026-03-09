namespace OperationIntelligence.Core;

public class AddShipmentPackageRequest
{
    public string PackageNumber { get; set; } = string.Empty;
    public string PackageType { get; set; } = string.Empty;

    public decimal Length { get; set; }
    public decimal Width { get; set; }
    public decimal Height { get; set; }
    public decimal Weight { get; set; }
    public decimal DeclaredValue { get; set; }

    public bool RequiresSpecialHandling { get; set; }
    public bool IsFragile { get; set; }

    public string? TrackingNumber { get; set; }
    public string? Barcode { get; set; }
    public string? LabelUrl { get; set; }
}
