using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class ShipmentResponse
{
    public Guid Id { get; set; }
    public string ShipmentNumber { get; set; } = string.Empty;

    public Guid? OrderId { get; set; }
    public string? OrderNumber { get; set; }

    public Guid WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;

    public Guid? CarrierId { get; set; }
    public string? CarrierName { get; set; }

    public Guid? CarrierServiceId { get; set; }
    public string? CarrierServiceName { get; set; }

    public ShipmentAddressResponse OriginAddress { get; set; } = new();
    public ShipmentAddressResponse DestinationAddress { get; set; } = new();

    public ShipmentType Type { get; set; }
    public ShipmentStatus Status { get; set; }
    public ShipmentPriority Priority { get; set; }

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

    public DeliveryRunResponse? DeliveryRun { get; set; }
    public DockAppointmentResponse? DockAppointment { get; set; }

    public List<ShipmentItemResponse> Items { get; set; } = new();
    public List<ShipmentPackageResponse> Packages { get; set; } = new();
    public List<ShipmentTrackingEventResponse> TrackingEvents { get; set; } = new();
    public List<ShipmentDocumentResponse> Documents { get; set; } = new();
    public List<ShipmentChargeResponse> Charges { get; set; } = new();
    public List<ShipmentExceptionResponse> Exceptions { get; set; } = new();
    public List<ShipmentInsuranceResponse> Insurances { get; set; } = new();
    public List<ShipmentStatusHistoryResponse> StatusHistories { get; set; } = new();
    public List<CustomsDocumentResponse> CustomsDocuments { get; set; } = new();
    public List<ReturnShipmentResponse> ReturnShipments { get; set; } = new();

    public DateTime CreatedAtUtc { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public string? UpdatedBy { get; set; }
}
