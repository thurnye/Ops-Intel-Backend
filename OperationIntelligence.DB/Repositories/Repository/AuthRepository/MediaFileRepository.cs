using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class MediaFileRepository : BaseRepository<MediaFile>, IMediaFileRepository
{
    public MediaFileRepository(OperationIntelligenceDbContext context) : base(context)
    {
    }

    public async Task<MediaFile?> GetPublicFileAsync(Guid fileId, CancellationToken cancellationToken = default)
    {
        return await _context.MediaFiles
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == fileId && x.IsPublic, cancellationToken);
    }

    public async Task<IReadOnlyList<MediaFile>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        var idList = ids.Distinct().ToList();

        return await _context.MediaFiles
            .AsNoTracking()
            .Where(x => idList.Contains(x.Id))
            .ToListAsync(cancellationToken);
    }
}
