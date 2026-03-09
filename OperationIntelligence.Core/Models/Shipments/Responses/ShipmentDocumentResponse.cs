using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class ShipmentDocumentResponse
{
    public Guid Id { get; set; }
    public ShipmentDocumentType DocumentType { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string? ContentType { get; set; }
    public long FileSizeBytes { get; set; }
    public bool IsCustomerVisible { get; set; }
    public string? Notes { get; set; }
}
