namespace OperationIntelligence.Core;

public class CreateOrderAddressRequest
{
    public Guid OrderId { get; set; }
    public AddressType AddressType { get; set; }
    public string ContactName { get; set; } = default!;
    public string? CompanyName { get; set; }
    public string AddressLine1 { get; set; } = default!;
    public string? AddressLine2 { get; set; }
    public string City { get; set; } = default!;
    public string StateOrProvince { get; set; } = default!;
    public string PostalCode { get; set; } = default!;
    public string Country { get; set; } = default!;
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
}