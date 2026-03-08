
namespace OperationIntelligence.DB;

public interface IProductImageRepository : IBaseRepository<ProductImage>
{
    Task<IReadOnlyList<ProductImage>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<ProductImage?> GetPrimaryImageAsync(Guid productId, CancellationToken cancellationToken = default);
}
