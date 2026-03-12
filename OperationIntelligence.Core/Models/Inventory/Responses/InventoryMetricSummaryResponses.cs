namespace OperationIntelligence.Core;

public class ProductMetricsSummaryResponse
{
    public int TotalProducts { get; set; }
    public int ActiveProducts { get; set; }
    public int DraftProducts { get; set; }
    public int InactiveProducts { get; set; }
    public int DiscontinuedProducts { get; set; }
}

public class BrandMetricsSummaryResponse
{
    public int TotalBrands { get; set; }
    public int BrandsWithDescriptions { get; set; }
    public int DescriptionCoveragePercentage { get; set; }
}

public class WarehouseMetricsSummaryResponse
{
    public int TotalWarehouses { get; set; }
    public int ActiveWarehouses { get; set; }
    public int CountriesRepresented { get; set; }
    public int AddressReadyWarehouses { get; set; }
}

public class SupplierMetricsSummaryResponse
{
    public int TotalSuppliers { get; set; }
    public int ActiveSuppliers { get; set; }
    public int ContactableSuppliers { get; set; }
    public int CountriesRepresented { get; set; }
}
