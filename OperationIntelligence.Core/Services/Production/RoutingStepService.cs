using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class RoutingStepService : IRoutingStepService
{
    private readonly IRoutingStepRepository _routingStepRepository;

    public RoutingStepService(IRoutingStepRepository routingStepRepository)
    {
        _routingStepRepository = routingStepRepository;
    }

    public async Task<IReadOnlyList<RoutingStepResponse>> GetByRoutingIdAsync(Guid routingId, CancellationToken cancellationToken = default)
    {
        var items = await _routingStepRepository.GetByRoutingIdAsync(routingId, cancellationToken);
        return items.Select(x => x.ToResponse()).ToList();
    }

    public async Task<RoutingStepResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _routingStepRepository.GetByIdAsync(id, cancellationToken);
        return entity is null || entity.IsDeleted ? null : entity.ToResponse();
    }

    public async Task<bool> DeleteAsync(Guid id, string? deletedBy = null, CancellationToken cancellationToken = default)
    {
        var entity = await _routingStepRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null || entity.IsDeleted) return false;

        entity.IsDeleted = true;
        entity.DeletedAtUtc = DateTime.UtcNow;
        entity.DeletedBy = deletedBy;

        _routingStepRepository.Update(entity);
        await _routingStepRepository.SaveChangesAsync(cancellationToken);
        return true;
    }
}
