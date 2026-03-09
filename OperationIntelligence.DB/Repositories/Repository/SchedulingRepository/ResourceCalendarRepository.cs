using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class ResourceCalendarRepository : IResourceCalendarRepository
{
    private readonly OperationIntelligenceDbContext _context;

    public ResourceCalendarRepository(OperationIntelligenceDbContext context)
    {
        _context = context;
    }

    public async Task<ResourceCalendar?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ResourceCalendars
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }

    public async Task<ResourceCalendar?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ResourceCalendars
            .AsNoTracking()
            .Include(x => x.Exceptions.Where(e => !e.IsDeleted))
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }

    public async Task<ResourceCalendar?> GetDefaultAsync(Guid resourceId, ResourceType resourceType, CancellationToken cancellationToken = default)
    {
        return await _context.ResourceCalendars
            .AsNoTracking()
            .Include(x => x.Exceptions.Where(e => !e.IsDeleted))
            .FirstOrDefaultAsync(x => x.ResourceId == resourceId && x.ResourceType == resourceType && x.IsDefault && !x.IsDeleted, cancellationToken);
    }

    public async Task<IReadOnlyList<ResourceCalendar>> GetByResourceAsync(Guid resourceId, ResourceType resourceType, CancellationToken cancellationToken = default)
    {
        return await _context.ResourceCalendars
            .AsNoTracking()
            .Where(x => x.ResourceId == resourceId && x.ResourceType == resourceType && !x.IsDeleted)
            .OrderByDescending(x => x.IsDefault)
            .ThenBy(x => x.CalendarName)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(ResourceCalendar entity, CancellationToken cancellationToken = default)
    {
        await _context.ResourceCalendars.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(ResourceCalendar entity, CancellationToken cancellationToken = default)
    {
        _context.ResourceCalendars.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(ResourceCalendar entity, CancellationToken cancellationToken = default)
    {
        _context.ResourceCalendars.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
