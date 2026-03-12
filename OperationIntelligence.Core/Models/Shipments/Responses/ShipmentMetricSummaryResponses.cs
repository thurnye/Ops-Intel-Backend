namespace OperationIntelligence.Core;

public class CarrierMetricsSummaryResponse
{
    public int TotalCarriers { get; set; }
    public int ActiveCarriers { get; set; }
    public int ContactableCarriers { get; set; }
    public int TotalServices { get; set; }
}

public class ShipmentAddressMetricsSummaryResponse
{
    public int TotalAddresses { get; set; }
    public int CountriesRepresented { get; set; }
    public int CitiesRepresented { get; set; }
    public int CompanyBackedAddresses { get; set; }
}
