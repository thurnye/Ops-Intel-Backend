using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class ScheduleRevisionRepository : IScheduleRevisionRepository
{
    private readonly OperationIntelligenceDbContext _context;

    public ScheduleRevisionRepository(OperationIntelligenceDbContext context)
    {
        _context = context;
    }

    public async Task<ScheduleRevision?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ScheduleRevisions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }

    public async Task<IReadOnlyList<ScheduleRevision>> GetBySchedulePlanIdAsync(Guid schedulePlanId, CancellationToken cancellationToken = default)
    {
        return await _context.ScheduleRevisions
            .AsNoTracking()
            .Where(x => x.SchedulePlanId == schedulePlanId && !x.IsDeleted)
            .OrderByDescending(x => x.RevisionNo)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(ScheduleRevision entity, CancellationToken cancellationToken = default)
    {
        await _context.ScheduleRevisions.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsRevisionNumberAsync(Guid schedulePlanId, int revisionNo, CancellationToken cancellationToken = default)
    {
        return await _context.ScheduleRevisions.AnyAsync(
            x => x.SchedulePlanId == schedulePlanId && x.RevisionNo == revisionNo && !x.IsDeleted,
            cancellationToken);
    }
}
