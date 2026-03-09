namespace OperationIntelligence.DB;
public interface IDispatchQueueRepository
{
    Task<DispatchQueueItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DispatchQueueItem>> GetByWorkCenterAsync(Guid workCenterId, Guid? machineId = null, bool activeOnly = true, CancellationToken cancellationToken = default);

    Task AddAsync(DispatchQueueItem entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(DispatchQueueItem entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(DispatchQueueItem entity, CancellationToken cancellationToken = default);

    Task<bool> ExistsQueuePositionAsync(Guid workCenterId, Guid? machineId, int queuePosition, Guid? excludeId = null, CancellationToken cancellationToken = default);
}
