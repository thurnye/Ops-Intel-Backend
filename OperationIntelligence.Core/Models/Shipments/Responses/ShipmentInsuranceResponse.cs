using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class ShipmentInsuranceResponse
{
    public Guid Id { get; set; }
    public string ProviderName { get; set; } = string.Empty;
    public string? PolicyNumber { get; set; }
    public decimal InsuredAmount { get; set; }
    public decimal PremiumAmount { get; set; }
    public string CurrencyCode { get; set; } = "CAD";
    public DateTime EffectiveDateUtc { get; set; }
    public DateTime? ExpiryDateUtc { get; set; }
    public InsuranceStatus Status { get; set; }
    public string? Notes { get; set; }
}
