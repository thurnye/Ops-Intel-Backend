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
        string? search = null,
        DateTime? startDateUtc = null,
        DateTime? endDateUtc = null,
        Guid? warehouseId = null,
        int? status = null,
        int? generationMode = null,
        int? schedulingStrategy = null,
        bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        pageNumber = pageNumber <= 0 ? 1 : pageNumber;
        pageSize = pageSize <= 0 ? 20 : pageSize;

        var query = _context.SchedulePlans
            .AsNoTracking()
            .Include(x => x.Warehouse)
            .Where(x => !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(x =>
                x.PlanNumber.Contains(term) ||
                x.Name.Contains(term) ||
                (x.Description != null && x.Description.Contains(term)));
        }

        if (startDateUtc.HasValue)
        {
            query = query.Where(x => x.PlanningEndDateUtc >= startDateUtc.Value);
        }

        if (endDateUtc.HasValue)
        {
            query = query.Where(x => x.PlanningStartDateUtc <= endDateUtc.Value);
        }

        if (warehouseId.HasValue)
        {
            query = query.Where(x => x.WarehouseId == warehouseId.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(x => (int)x.Status == status.Value);
        }

        if (generationMode.HasValue)
        {
            query = query.Where(x => (int)x.GenerationMode == generationMode.Value);
        }

        if (schedulingStrategy.HasValue)
        {
            query = query.Where(x => (int)x.SchedulingStrategy == schedulingStrategy.Value);
        }

        if (isActive.HasValue)
        {
            query = query.Where(x => x.IsActive == isActive.Value);
        }

        query = query.OrderByDescending(x => x.CreatedAtUtc);

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
