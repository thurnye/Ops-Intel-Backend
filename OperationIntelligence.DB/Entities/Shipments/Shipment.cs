namespace OperationIntelligence.DB;

public class Shipment : AuditableEntity
{
    public string ShipmentNumber { get; set; } = string.Empty;

    public Guid? OrderId { get; set; }
    public Order? Order { get; set; }

    public Guid WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = default!;

    public Guid? CarrierId { get; set; }
    public Carrier? Carrier { get; set; }

    public Guid? CarrierServiceId { get; set; }
    public CarrierService? CarrierService { get; set; }

    public Guid OriginAddressId { get; set; }
    public ShipmentAddress OriginAddress { get; set; } = default!;

    public Guid DestinationAddressId { get; set; }
    public ShipmentAddress DestinationAddress { get; set; } = default!;

    public Guid? DeliveryRunId { get; set; }
    public DeliveryRun? DeliveryRun { get; set; }

    public Guid? DockAppointmentId { get; set; }
    public DockAppointment? DockAppointment { get; set; }

    public ShipmentType Type { get; set; } = ShipmentType.Outbound;
    public ShipmentStatus Status { get; set; } = ShipmentStatus.Draft;
    public ShipmentPriority Priority { get; set; } = ShipmentPriority.Normal;

    public string? CustomerReference { get; set; }
    public string? ExternalReference { get; set; }
    public string? TrackingNumber { get; set; }
    public string? MasterTrackingNumber { get; set; }

    public DateTime? PlannedShipDateUtc { get; set; }
    public DateTime? PlannedDeliveryDateUtc { get; set; }
    public DateTime? ActualShipDateUtc { get; set; }
    public DateTime? ActualDeliveryDateUtc { get; set; }

    public DateTime? ScheduledPickupStartUtc { get; set; }
    public DateTime? ScheduledPickupEndUtc { get; set; }

    public decimal TotalWeight { get; set; }
    public decimal TotalVolume { get; set; }
    public int TotalPackages { get; set; }

    public decimal FreightCost { get; set; }
    public decimal InsuranceCost { get; set; }
    public decimal OtherCharges { get; set; }
    public decimal TotalShippingCost { get; set; }

    public string CurrencyCode { get; set; } = "CAD";
    public string? ShippingTerms { get; set; }
    public string? Incoterm { get; set; }

    public bool IsPartialShipment { get; set; }
    public bool RequiresSignature { get; set; }
    public bool IsFragile { get; set; }
    public bool IsHazardous { get; set; }
    public bool IsTemperatureControlled { get; set; }
    public bool IsInsured { get; set; }
    public bool IsCrossBorder { get; set; }

    public string? Notes { get; set; }
    public string? InternalNotes { get; set; }

    public ICollection<ShipmentItem> Items { get; set; } = new List<ShipmentItem>();
    public ICollection<ShipmentPackage> Packages { get; set; } = new List<ShipmentPackage>();
    public ICollection<ShipmentTrackingEvent> TrackingEvents { get; set; } = new List<ShipmentTrackingEvent>();
    public ICollection<ShipmentDocument> Documents { get; set; } = new List<ShipmentDocument>();
    public ICollection<ShipmentStatusHistory> StatusHistories { get; set; } = new List<ShipmentStatusHistory>();
    public ICollection<ShipmentException> Exceptions { get; set; } = new List<ShipmentException>();
    public ICollection<ShipmentCharge> Charges { get; set; } = new List<ShipmentCharge>();
    public ICollection<ShipmentInsurance> Insurances { get; set; } = new List<ShipmentInsurance>();
    public ICollection<CustomsDocument> CustomsDocuments { get; set; } = new List<CustomsDocument>();
    public ICollection<ReturnShipment> ReturnShipments { get; set; } = new List<ReturnShipment>();
}