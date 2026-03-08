namespace OperationIntelligence.Core;

public interface IProductImageService
{
    Task<ProductImageResponse> AddAsync(AddProductImageRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ProductImageResponse>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid imageId, CancellationToken cancellationToken = default);
    Task<bool> SetPrimaryAsync(Guid imageId, CancellationToken cancellationToken = default);
}
