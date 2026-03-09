namespace OperationIntelligence.Core;

public class ShipmentSummaryResponse
{
    public int TotalShipments { get; set; }
    public int DraftShipments { get; set; }
    public int ReadyToDispatchShipments { get; set; }
    public int InTransitShipments { get; set; }
    public int DeliveredShipments { get; set; }
    public int FailedShipments { get; set; }
    public int ReturnedShipments { get; set; }

    public decimal TotalFreightCost { get; set; }
    public decimal TotalShippingCost { get; set; }

    public int TotalPackages { get; set; }
    public decimal TotalWeight { get; set; }
}
