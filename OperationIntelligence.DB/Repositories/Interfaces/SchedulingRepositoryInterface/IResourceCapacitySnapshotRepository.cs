namespace OperationIntelligence.DB;


public interface IResourceCapacitySnapshotRepository
{
    Task<ResourceCapacitySnapshot?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<ResourceCapacitySnapshot> Items, int TotalRecords)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task AddAsync(ResourceCapacitySnapshot entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(ResourceCapacitySnapshot entity, CancellationToken cancellationToken = default);
}
