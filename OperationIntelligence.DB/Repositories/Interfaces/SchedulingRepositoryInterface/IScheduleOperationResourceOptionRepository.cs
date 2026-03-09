namespace OperationIntelligence.DB;


public interface IScheduleOperationResourceOptionRepository
{
    Task<ScheduleOperationResourceOption?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ScheduleOperationResourceOption>> GetByOperationIdAsync(Guid operationId, CancellationToken cancellationToken = default);

    Task AddAsync(ScheduleOperationResourceOption entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(ScheduleOperationResourceOption entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(ScheduleOperationResourceOption entity, CancellationToken cancellationToken = default);
}
