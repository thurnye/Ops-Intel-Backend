namespace OperationIntelligence.DB;

public class ShipmentAddress : AuditableEntity
{
    public string AddressType { get; set; } = string.Empty;

    public string ContactName { get; set; } = string.Empty;
    public string? CompanyName { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }

    public string AddressLine1 { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
    public string City { get; set; } = string.Empty;
    public string StateOrProvince { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;

    public ICollection<Shipment> OriginShipments { get; set; } = new List<Shipment>();
    public ICollection<Shipment> DestinationShipments { get; set; } = new List<Shipment>();
    public ICollection<ReturnShipment> OriginReturnShipments { get; set; } = new List<ReturnShipment>();
    public ICollection<ReturnShipment> DestinationReturnShipments { get; set; } = new List<ReturnShipment>();
}