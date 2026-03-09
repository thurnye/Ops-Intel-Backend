namespace OperationIntelligence.DB;
public interface IResourceCalendarExceptionRepository
{
    Task<ResourceCalendarException?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ResourceCalendarException>> GetByCalendarIdAsync(Guid resourceCalendarId, CancellationToken cancellationToken = default);

    Task AddAsync(ResourceCalendarException entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(ResourceCalendarException entity, CancellationToken cancellationToken = default);
}
