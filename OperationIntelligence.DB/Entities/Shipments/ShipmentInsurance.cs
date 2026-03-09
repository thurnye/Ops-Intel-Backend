namespace OperationIntelligence.DB;

public class ShipmentInsurance : AuditableEntity
{
    public Guid ShipmentId { get; set; }
    public Shipment Shipment { get; set; } = default!;

    public string ProviderName { get; set; } = string.Empty;
    public string? PolicyNumber { get; set; }

    public decimal InsuredAmount { get; set; }
    public decimal PremiumAmount { get; set; }
    public string CurrencyCode { get; set; } = "CAD";

    public DateTime EffectiveDateUtc { get; set; }
    public DateTime? ExpiryDateUtc { get; set; }

    public InsuranceStatus Status { get; set; } = InsuranceStatus.Pending;
    public string? Notes { get; set; }
}