using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class ShipmentChargeResponse
{
    public Guid Id { get; set; }
    public ShipmentChargeType ChargeType { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string CurrencyCode { get; set; } = "CAD";
}
