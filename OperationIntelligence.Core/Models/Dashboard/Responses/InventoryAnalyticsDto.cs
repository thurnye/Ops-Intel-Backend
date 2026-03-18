namespace OperationIntelligence.Core;

public class InventoryAnalyticsDto
{
    public List<ChartPointDto> TopLowStockItems { get; set; } = new();
    public List<MultiSeriesPointDto> InventoryInflowOutflow { get; set; } = new();
    public List<WarehouseStockCompositionDto> WarehouseStockComposition { get; set; } = new();
    public List<PieSliceDto> InventoryMixByCategory { get; set; } = new();
}

public class WarehouseStockCompositionDto
{
    public string Warehouse { get; set; } = string.Empty;
    public decimal RawMaterials { get; set; }
    public decimal FinishedGoods { get; set; }
    public decimal Packaging { get; set; }
}