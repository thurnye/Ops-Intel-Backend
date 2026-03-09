using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class ScheduleOperationDependencyRepository : IScheduleOperationDependencyRepository
{
    private readonly OperationIntelligenceDbContext _context;

    public ScheduleOperationDependencyRepository(OperationIntelligenceDbContext context)
    {
        _context = context;
    }

    public async Task<ScheduleOperationDependency?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ScheduleOperationDependencies
            .AsNoTracking()
            .Include(x => x.PredecessorOperation)
            .Include(x => x.SuccessorOperation)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }

    public async Task<IReadOnlyList<ScheduleOperationDependency>> GetByOperationIdAsync(Guid operationId, CancellationToken cancellationToken = default)
    {
        return await _context.ScheduleOperationDependencies
            .AsNoTracking()
            .Include(x => x.PredecessorOperation)
            .Include(x => x.SuccessorOperation)
            .Where(x => !x.IsDeleted && (x.PredecessorOperationId == operationId || x.SuccessorOperationId == operationId))
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(ScheduleOperationDependency entity, CancellationToken cancellationToken = default)
    {
        await _context.ScheduleOperationDependencies.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(ScheduleOperationDependency entity, CancellationToken cancellationToken = default)
    {
        _context.ScheduleOperationDependencies.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid predecessorOperationId, Guid successorOperationId, CancellationToken cancellationToken = default)
    {
        return await _context.ScheduleOperationDependencies.AnyAsync(
            x => !x.IsDeleted && x.PredecessorOperationId == predecessorOperationId && x.SuccessorOperationId == successorOperationId,
            cancellationToken);
    }
}
