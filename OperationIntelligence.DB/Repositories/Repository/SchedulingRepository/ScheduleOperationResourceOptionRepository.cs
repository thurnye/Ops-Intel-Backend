using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class ScheduleOperationResourceOptionRepository : IScheduleOperationResourceOptionRepository
{
    private readonly OperationIntelligenceDbContext _context;

    public ScheduleOperationResourceOptionRepository(OperationIntelligenceDbContext context)
    {
        _context = context;
    }

    public async Task<ScheduleOperationResourceOption?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ScheduleOperationResourceOptions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }

    public async Task<IReadOnlyList<ScheduleOperationResourceOption>> GetByOperationIdAsync(Guid operationId, CancellationToken cancellationToken = default)
    {
        return await _context.ScheduleOperationResourceOptions
            .AsNoTracking()
            .Where(x => x.ScheduleOperationId == operationId && !x.IsDeleted)
            .OrderBy(x => x.PreferenceRank)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(ScheduleOperationResourceOption entity, CancellationToken cancellationToken = default)
    {
        await _context.ScheduleOperationResourceOptions.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(ScheduleOperationResourceOption entity, CancellationToken cancellationToken = default)
    {
        _context.ScheduleOperationResourceOptions.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(ScheduleOperationResourceOption entity, CancellationToken cancellationToken = default)
    {
        _context.ScheduleOperationResourceOptions.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
