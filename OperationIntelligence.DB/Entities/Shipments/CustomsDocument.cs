namespace OperationIntelligence.DB;

public class CustomsDocument : AuditableEntity
{
    public Guid ShipmentId { get; set; }
    public Shipment Shipment { get; set; } = default!;

    public CustomsDocumentType DocumentType { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;

    public string? CountryOfOrigin { get; set; }
    public string? DestinationCountry { get; set; }
    public string? HarmonizedCode { get; set; }
    public decimal? DeclaredCustomsValue { get; set; }
    public string CurrencyCode { get; set; } = "CAD";

    public DateTime IssuedAtUtc { get; set; }
    public string? Notes { get; set; }
}