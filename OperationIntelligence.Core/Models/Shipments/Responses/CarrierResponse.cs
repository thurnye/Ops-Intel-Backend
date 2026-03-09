namespace OperationIntelligence.Core;

public class CarrierResponse
{
    public Guid Id { get; set; }
    public string CarrierCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? ContactName { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public bool IsActive { get; set; }
    public List<CarrierServiceResponse> Services { get; set; } = new();
}
