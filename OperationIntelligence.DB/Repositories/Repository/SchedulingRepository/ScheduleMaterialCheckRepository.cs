using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class ScheduleMaterialCheckRepository : IScheduleMaterialCheckRepository
{
    private readonly OperationIntelligenceDbContext _context;

    public ScheduleMaterialCheckRepository(OperationIntelligenceDbContext context)
    {
        _context = context;
    }

    public async Task<ScheduleMaterialCheck?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ScheduleMaterialChecks
            .AsNoTracking()
            .Include(x => x.MaterialProduct)
            .Include(x => x.Warehouse)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }

    public async Task<IReadOnlyList<ScheduleMaterialCheck>> GetByScheduleJobIdAsync(Guid scheduleJobId, CancellationToken cancellationToken = default)
    {
        return await _context.ScheduleMaterialChecks
            .AsNoTracking()
            .Include(x => x.MaterialProduct)
            .Include(x => x.Warehouse)
            .Where(x => x.ScheduleJobId == scheduleJobId && !x.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ScheduleMaterialCheck>> GetByScheduleOperationIdAsync(Guid scheduleOperationId, CancellationToken cancellationToken = default)
    {
        return await _context.ScheduleMaterialChecks
            .AsNoTracking()
            .Include(x => x.MaterialProduct)
            .Include(x => x.Warehouse)
            .Where(x => x.ScheduleOperationId == scheduleOperationId && !x.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(ScheduleMaterialCheck entity, CancellationToken cancellationToken = default)
    {
        await _context.ScheduleMaterialChecks.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(ScheduleMaterialCheck entity, CancellationToken cancellationToken = default)
    {
        _context.ScheduleMaterialChecks.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(ScheduleMaterialCheck entity, CancellationToken cancellationToken = default)
    {
        _context.ScheduleMaterialChecks.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
