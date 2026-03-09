using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class ResourceCalendarExceptionRepository : IResourceCalendarExceptionRepository
{
    private readonly OperationIntelligenceDbContext _context;

    public ResourceCalendarExceptionRepository(OperationIntelligenceDbContext context)
    {
        _context = context;
    }

    public async Task<ResourceCalendarException?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ResourceCalendarExceptions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }

    public async Task<IReadOnlyList<ResourceCalendarException>> GetByCalendarIdAsync(Guid resourceCalendarId, CancellationToken cancellationToken = default)
    {
        return await _context.ResourceCalendarExceptions
            .AsNoTracking()
            .Where(x => x.ResourceCalendarId == resourceCalendarId && !x.IsDeleted)
            .OrderBy(x => x.ExceptionStartUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(ResourceCalendarException entity, CancellationToken cancellationToken = default)
    {
        await _context.ResourceCalendarExceptions.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(ResourceCalendarException entity, CancellationToken cancellationToken = default)
    {
        _context.ResourceCalendarExceptions.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
