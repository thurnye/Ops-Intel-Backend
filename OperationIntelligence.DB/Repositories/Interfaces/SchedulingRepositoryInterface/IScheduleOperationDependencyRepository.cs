namespace OperationIntelligence.DB;


public interface IScheduleOperationDependencyRepository
{
    Task<ScheduleOperationDependency?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ScheduleOperationDependency>> GetByOperationIdAsync(Guid operationId, CancellationToken cancellationToken = default);

    Task AddAsync(ScheduleOperationDependency entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(ScheduleOperationDependency entity, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(Guid predecessorOperationId, Guid successorOperationId, CancellationToken cancellationToken = default);
}
