namespace OperationIntelligence.Core;

public static class InventoryErrorMessages
{
    public const string ProductNotFound = "Product not found.";
    public const string SupplierNotFound = "Supplier not found.";
    public const string InventoryStockNotFound = "Inventory stock record not found.";
    public const string InsufficientAvailableStock = "Insufficient available stock.";
    public const string ProductCreatedButNotRetrieved = "Product was created but could not be retrieved.";
    public const string SupplierAlreadyAssignedToProduct = "This supplier is already assigned to the product.";

    public static string CategoryAlreadyExists(string name) => $"Category '{name}' already exists.";
    public static string BrandAlreadyExists(string name) => $"Brand '{name}' already exists.";
    public static string SupplierAlreadyExists(string name) => $"Supplier '{name}' already exists.";
    public static string WarehouseAlreadyExists(string name) => $"Warehouse '{name}' already exists.";
    public static string WarehouseCodeAlreadyExists(string code) => $"Warehouse code '{code}' already exists.";
    public static string UnitOfMeasureAlreadyExists(string name) => $"Unit of measure '{name}' already exists.";
    public static string UnitOfMeasureSymbolAlreadyExists(string symbol) => $"Unit of measure symbol '{symbol}' already exists.";
    public static string ProductSkuAlreadyExists(string sku) => $"A product with SKU '{sku}' already exists.";
    public static string ProductBarcodeAlreadyExists(string? barcode) => $"A product with barcode '{barcode}' already exists.";
}
