namespace OperationIntelligence.DB;

public class ShipmentDocument : AuditableEntity
{
    public Guid ShipmentId { get; set; }
    public Shipment Shipment { get; set; } = default!;

    public ShipmentDocumentType DocumentType { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string? ContentType { get; set; }
    public long FileSizeBytes { get; set; }

    public bool IsCustomerVisible { get; set; }
    public string? Notes { get; set; }
}