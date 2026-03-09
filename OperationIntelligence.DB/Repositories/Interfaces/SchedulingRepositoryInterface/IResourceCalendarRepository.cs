namespace OperationIntelligence.DB;
public interface IResourceCalendarRepository
{
    Task<ResourceCalendar?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ResourceCalendar?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ResourceCalendar?> GetDefaultAsync(Guid resourceId, ResourceType resourceType, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ResourceCalendar>> GetByResourceAsync(Guid resourceId, ResourceType resourceType, CancellationToken cancellationToken = default);

    Task AddAsync(ResourceCalendar entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(ResourceCalendar entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(ResourceCalendar entity, CancellationToken cancellationToken = default);
}
