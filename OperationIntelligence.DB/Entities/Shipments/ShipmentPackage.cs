namespace OperationIntelligence.DB;

public class ShipmentPackage : AuditableEntity
{
    public Guid ShipmentId { get; set; }
    public Shipment Shipment { get; set; } = default!;

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

    public PackageStatus Status { get; set; } = PackageStatus.Draft;

    public ICollection<ShipmentPackageItem> PackageItems { get; set; } = new List<ShipmentPackageItem>();
}