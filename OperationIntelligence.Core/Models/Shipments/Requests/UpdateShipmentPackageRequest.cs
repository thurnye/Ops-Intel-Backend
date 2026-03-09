using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class UpdateShipmentPackageRequest
{
    public string PackageType { get; set; } = string.Empty;
    public string? TrackingNumber { get; set; }

    public decimal Length { get; set; }
    public decimal Width { get; set; }
    public decimal Height { get; set; }
    public decimal Weight { get; set; }
    public decimal DeclaredValue { get; set; }

    public bool RequiresSpecialHandling { get; set; }
    public bool IsFragile { get; set; }

    public string? Barcode { get; set; }
    public string? LabelUrl { get; set; }

    public PackageStatus Status { get; set; }
}
