namespace OperationIntelligence.DB;

public class ShipmentTrackingEvent : AuditableEntity
{
    public Guid ShipmentId { get; set; }
    public Shipment Shipment { get; set; } = default!;

    public string EventCode { get; set; } = string.Empty;
    public string EventName { get; set; } = string.Empty;
    public string? Description { get; set; }

    public DateTime EventTimeUtc { get; set; }

    public string? LocationName { get; set; }
    public string? City { get; set; }
    public string? StateOrProvince { get; set; }
    public string? Country { get; set; }

    public string? CarrierStatusCode { get; set; }
    public string Source { get; set; } = string.Empty;
    public bool IsCustomerVisible { get; set; }
}