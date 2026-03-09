using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class SchedulePlanRepository : ISchedulePlanRepository
{
    private readonly OperationIntelligenceDbContext _context;

    public SchedulePlanRepository(OperationIntelligenceDbContext context)
    {
        _context = context;
    }

    public async Task<SchedulePlan?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.SchedulePlans
            .AsNoTracking()
            .Include(x => x.Warehouse)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }

    public async Task<SchedulePlan?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.SchedulePlans
            .AsNoTracking()
            .Include(x => x.Warehouse)
            .Include(x => x.ScheduleJobs)
                .ThenInclude(x => x.ScheduleOperations)
            .Include(x => x.ScheduleExceptions)
            .Include(x => x.ScheduleRevisions)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }

    public async Task<SchedulePlan?> GetByPlanNumberAsync(string planNumber, CancellationToken cancellationToken = default)
    {
        return await _context.SchedulePlans
            .AsNoTracking()
            .Include(x => x.Warehouse)
            .FirstOrDefaultAsync(x => x.PlanNumber == planNumber && !x.IsDeleted, cancellationToken);
    }

    public async Task<(IReadOnlyList<SchedulePlan> Items, int TotalRecords)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        pageNumber = pageNumber <= 0 ? 1 : pageNumber;
        pageSize = pageSize <= 0 ? 20 : pageSize;

        var query = _context.SchedulePlans
            .AsNoTracking()
            .Include(x => x.Warehouse)
            .Where(x => !x.IsDeleted)
            .OrderByDescending(x => x.CreatedAtUtc);

        var totalRecords = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalRecords);
    }

    public async Task AddAsync(SchedulePlan entity, CancellationToken cancellationToken = default)
    {
        await _context.SchedulePlans.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(SchedulePlan entity, CancellationToken cancellationToken = default)
    {
        _context.SchedulePlans.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(SchedulePlan entity, CancellationToken cancellationToken = default)
    {
        _context.SchedulePlans.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsByPlanNumberAsync(string planNumber, CancellationToken cancellationToken = default)
    {
        return await _context.SchedulePlans.AnyAsync(x => x.PlanNumber == planNumber && !x.IsDeleted, cancellationToken);
    }
}
