using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class ScheduleOperationConstraintRepository : IScheduleOperationConstraintRepository
{
    private readonly OperationIntelligenceDbContext _context;

    public ScheduleOperationConstraintRepository(OperationIntelligenceDbContext context)
    {
        _context = context;
    }

    public async Task<ScheduleOperationConstraint?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ScheduleOperationConstraints
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }

    public async Task<IReadOnlyList<ScheduleOperationConstraint>> GetByOperationIdAsync(Guid operationId, CancellationToken cancellationToken = default)
    {
        return await _context.ScheduleOperationConstraints
            .AsNoTracking()
            .Where(x => x.ScheduleOperationId == operationId && !x.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(ScheduleOperationConstraint entity, CancellationToken cancellationToken = default)
    {
        await _context.ScheduleOperationConstraints.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(ScheduleOperationConstraint entity, CancellationToken cancellationToken = default)
    {
        _context.ScheduleOperationConstraints.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(ScheduleOperationConstraint entity, CancellationToken cancellationToken = default)
    {
        _context.ScheduleOperationConstraints.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
