namespace OperationIntelligence.DB;


public interface IScheduleRevisionRepository
{
    Task<ScheduleRevision?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ScheduleRevision>> GetBySchedulePlanIdAsync(Guid schedulePlanId, CancellationToken cancellationToken = default);

    Task AddAsync(ScheduleRevision entity, CancellationToken cancellationToken = default);

    Task<bool> ExistsRevisionNumberAsync(Guid schedulePlanId, int revisionNo, CancellationToken cancellationToken = default);
}
