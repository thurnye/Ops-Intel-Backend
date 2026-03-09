namespace OperationIntelligence.DB;

public interface IRoutingRepository : IBaseRepository<Routing>
{
    Task<Routing?> GetWithStepsAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Routing?> GetDefaultByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Routing>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);

    Task<Routing?> GetActiveVersionAsync(Guid productId, int version, CancellationToken cancellationToken = default);

    Task<bool> RoutingCodeExistsAsync(string routingCode, Guid? excludeId = null, CancellationToken cancellationToken = default);
}
