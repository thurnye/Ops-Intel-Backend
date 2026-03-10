namespace OperationIntelligence.DB;

public interface IScheduleExceptionRepository
{
    Task<ScheduleException?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<ScheduleException> Items, int TotalRecords)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? search = null,
        DateTime? startDateUtc = null,
        DateTime? endDateUtc = null,
        Guid? schedulePlanId = null,
        Guid? scheduleJobId = null,
        Guid? scheduleOperationId = null,
        int? exceptionType = null,
        int? severity = null,
        int? status = null,
        string? assignedTo = null,
        CancellationToken cancellationToken = default);

    Task AddAsync(ScheduleException entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(ScheduleException entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(ScheduleException entity, CancellationToken cancellationToken = default);
}
