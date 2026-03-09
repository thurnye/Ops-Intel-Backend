namespace OperationIntelligence.DB;

public interface IRoutingStepRepository : IBaseRepository<RoutingStep>
{
    Task<IReadOnlyList<RoutingStep>> GetByRoutingIdAsync(Guid routingId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RoutingStep>> GetByWorkCenterIdAsync(Guid workCenterId, CancellationToken cancellationToken = default);

    Task<RoutingStep?> GetByRoutingAndSequenceAsync(Guid routingId, int sequence, CancellationToken cancellationToken = default);
}
