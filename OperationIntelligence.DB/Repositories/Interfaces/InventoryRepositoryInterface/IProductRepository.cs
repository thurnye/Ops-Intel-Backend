namespace OperationIntelligence.DB;


public interface IProductRepository : IBaseRepository<Product>
{
    Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default);
    Task<Product?> GetByBarcodeAsync(string barcode, CancellationToken cancellationToken = default);
    Task<Product?> GetProductWithImagesAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<Product?> GetProductWithDetailsAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Product>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        Guid? categoryId = null,
        ProductStatus? status = null,
        CancellationToken cancellationToken = default);

    Task<int> CountAsync(
        string? searchTerm = null,
        Guid? categoryId = null,
        ProductStatus? status = null,
        CancellationToken cancellationToken = default);
    Task<bool> IsSkuUniqueAsync(string sku, Guid? excludeProductId = null, CancellationToken cancellationToken = default);
    Task<bool> IsBarcodeUniqueAsync(string? barcode, Guid? excludeProductId = null, CancellationToken cancellationToken = default);
}
