namespace OperationIntelligence.DB;


public interface ISchedulePlanRepository
{
    Task<SchedulePlan?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<SchedulePlan?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<SchedulePlan?> GetByPlanNumberAsync(string planNumber, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<SchedulePlan> Items, int TotalRecords)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task AddAsync(SchedulePlan entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(SchedulePlan entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(SchedulePlan entity, CancellationToken cancellationToken = default);

    Task<bool> ExistsByPlanNumberAsync(string planNumber, CancellationToken cancellationToken = default);
}
