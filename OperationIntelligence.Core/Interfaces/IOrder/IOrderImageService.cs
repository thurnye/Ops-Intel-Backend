namespace OperationIntelligence.Core;

public interface IOrderImageService
{
    Task<OrderImageResponse> AddAsync(CreateOrderImageRequest request, CancellationToken cancellationToken = default);
    Task<OrderImageResponse> SetPrimaryAsync(Guid imageId, CancellationToken cancellationToken = default);
    Task RemoveAsync(Guid imageId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OrderImageResponse>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
}