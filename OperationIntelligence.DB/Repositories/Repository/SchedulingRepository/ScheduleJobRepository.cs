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
        string? search = null,
        DateTime? startDateUtc = null,
        DateTime? endDateUtc = null,
        Guid? schedulePlanId = null,
        Guid? productionOrderId = null,
        Guid? orderId = null,
        Guid? productId = null,
        Guid? warehouseId = null,
        int? status = null,
        int? priority = null,
        bool? materialsReady = null,
        int? materialReadinessStatus = null,
        bool? qualityHold = null,
        bool? isRushOrder = null,
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
            .Where(x => !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(x =>
                x.JobNumber.Contains(term) ||
                x.JobName.Contains(term) ||
                x.ProductionOrder.ProductionOrderNumber.Contains(term) ||
                (x.Order != null && x.Order.OrderNumber.Contains(term)) ||
                x.Product.Name.Contains(term) ||
                x.Product.SKU.Contains(term));
        }

        if (startDateUtc.HasValue)
        {
            query = query.Where(x => x.PlannedEndUtc >= startDateUtc.Value);
        }

        if (endDateUtc.HasValue)
        {
            query = query.Where(x => x.PlannedStartUtc <= endDateUtc.Value);
        }

        if (schedulePlanId.HasValue)
        {
            query = query.Where(x => x.SchedulePlanId == schedulePlanId.Value);
        }

        if (productionOrderId.HasValue)
        {
            query = query.Where(x => x.ProductionOrderId == productionOrderId.Value);
        }

        if (orderId.HasValue)
        {
            query = query.Where(x => x.OrderId == orderId.Value);
        }

        if (productId.HasValue)
        {
            query = query.Where(x => x.ProductId == productId.Value);
        }

        if (warehouseId.HasValue)
        {
            query = query.Where(x => x.WarehouseId == warehouseId.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(x => (int)x.Status == status.Value);
        }

        if (priority.HasValue)
        {
            query = query.Where(x => (int)x.Priority == priority.Value);
        }

        if (materialsReady.HasValue)
        {
            query = query.Where(x => x.MaterialsReady == materialsReady.Value);
        }

        if (materialReadinessStatus.HasValue)
        {
            query = query.Where(x => (int)x.MaterialReadinessStatus == materialReadinessStatus.Value);
        }

        if (qualityHold.HasValue)
        {
            query = query.Where(x => x.QualityHold == qualityHold.Value);
        }

        if (isRushOrder.HasValue)
        {
            query = query.Where(x => x.IsRushOrder == isRushOrder.Value);
        }

        query = query.OrderByDescending(x => x.CreatedAtUtc);

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
