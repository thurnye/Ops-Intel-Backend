namespace OperationIntelligence.DB;

public interface IOrderImageRepository : IOrderBaseRepository<OrderImage>
{
    Task<IReadOnlyList<OrderImage>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<OrderImage?> GetPrimaryImageByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
}