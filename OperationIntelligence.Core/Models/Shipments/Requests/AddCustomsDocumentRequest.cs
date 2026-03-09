using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class AddCustomsDocumentRequest
{
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
