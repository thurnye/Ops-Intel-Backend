namespace OperationIntelligence.DB;

public interface IScheduleExceptionRepository
{
    Task<ScheduleException?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<ScheduleException> Items, int TotalRecords)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task AddAsync(ScheduleException entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(ScheduleException entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(ScheduleException entity, CancellationToken cancellationToken = default);
}
