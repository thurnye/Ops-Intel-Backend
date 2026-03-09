using Microsoft.EntityFrameworkCore;
namespace OperationIntelligence.DB;

public class ScheduleJobRepository : IScheduleJobRepository
{
    private readonly OperationIntelligenceDbContext _context;

    public ScheduleJobRepository(OperationIntelligenceDbContext context)
    {
        _context = context;
    }

    public async Task<ScheduleJob?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ScheduleJobs
            .AsNoTracking()
            .Include(x => x.SchedulePlan)
            .Include(x => x.ProductionOrder)
            .Include(x => x.Order)
            .Include(x => x.OrderItem)
            .Include(x => x.Product)
            .Include(x => x.Warehouse)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }

    public async Task<ScheduleJob?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ScheduleJobs
            .AsNoTracking()
            .Include(x => x.SchedulePlan)
            .Include(x => x.ProductionOrder)
            .Include(x => x.Order)
            .Include(x => x.OrderItem)
            .Include(x => x.Product)
            .Include(x => x.Warehouse)
            .Include(x => x.ScheduleOperations)
            .Include(x => x.MaterialChecks)
                .ThenInclude(x => x.MaterialProduct)
            .Include(x => x.ScheduleExceptions)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }

    public async Task<ScheduleJob?> GetByJobNumberAsync(string jobNumber, CancellationToken cancellationToken = default)
    {
        return await _context.ScheduleJobs
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.JobNumber == jobNumber && !x.IsDeleted, cancellationToken);
    }

    public async Task<(IReadOnlyList<ScheduleJob> Items, int TotalRecords)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        pageNumber = pageNumber <= 0 ? 1 : pageNumber;
        pageSize = pageSize <= 0 ? 20 : pageSize;

        var query = _context.ScheduleJobs
            .AsNoTracking()
            .Include(x => x.ProductionOrder)
            .Include(x => x.Order)
            .Include(x => x.Product)
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

    public async Task AddAsync(ScheduleJob entity, CancellationToken cancellationToken = default)
    {
        await _context.ScheduleJobs.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(ScheduleJob entity, CancellationToken cancellationToken = default)
    {
        _context.ScheduleJobs.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(ScheduleJob entity, CancellationToken cancellationToken = default)
    {
        _context.ScheduleJobs.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsByJobNumberAsync(string jobNumber, CancellationToken cancellationToken = default)
    {
        return await _context.ScheduleJobs.AnyAsync(x => x.JobNumber == jobNumber && !x.IsDeleted, cancellationToken);
    }

    public async Task<bool> ExistsForProductionOrderAsync(Guid productionOrderId, CancellationToken cancellationToken = default)
    {
        return await _context.ScheduleJobs.AnyAsync(x => x.ProductionOrderId == productionOrderId && !x.IsDeleted, cancellationToken);
    }
}
