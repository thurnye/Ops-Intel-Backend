namespace OperationIntelligence.Core;

public interface IRoutingStepService
{
    Task<IReadOnlyList<RoutingStepResponse>> GetByRoutingIdAsync(Guid routingId, CancellationToken cancellationToken = default);
    Task<RoutingStepResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, string? deletedBy = null, CancellationToken cancellationToken = default);
}
