namespace OperationIntelligence.Core;

public interface IProductService
{
    Task<ProductResponse> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken = default);
    Task<ProductResponse?> GetByIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<PagedResponse<ProductListItemResponse>> GetPagedAsync(ProductQueryRequest request, CancellationToken cancellationToken = default);
    Task<ProductResponse?> UpdateAsync(UpdateProductRequest request, CancellationToken cancellationToken = default);
}
