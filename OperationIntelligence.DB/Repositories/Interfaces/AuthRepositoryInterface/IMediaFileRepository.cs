namespace OperationIntelligence.DB;

public interface IMediaFileRepository : IBaseRepository<MediaFile>
{
    Task<MediaFile?> GetPublicFileAsync(Guid fileId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MediaFile>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
}
