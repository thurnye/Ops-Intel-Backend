using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class ProductionOrderRepository : BaseRepository<ProductionOrder>, IProductionOrderRepository
{
    public ProductionOrderRepository(OperationIntelligenceDbContext context) : base(context)
    {
    }

    public async Task<ProductionOrder?> GetByProductionOrderNumberAsync(string productionOrderNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ProductionOrderNumber == productionOrderNumber && !x.IsDeleted, cancellationToken);
    }

    public async Task<ProductionOrder?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(x => x.Product)
            .Include(x => x.UnitOfMeasure)
            .Include(x => x.BillOfMaterial)
                .ThenInclude(x => x!.Items)
            .Include(x => x.Routing)
                .ThenInclude(x => x!.Steps)
            .Include(x => x.Warehouse)
            .Include(x => x.Executions)
                .ThenInclude(x => x.WorkCenter)
            .Include(x => x.Executions)
                .ThenInclude(x => x.Machine)
            .Include(x => x.MaterialIssues)
            .Include(x => x.Outputs)
            .Include(x => x.Scraps)
            .Include(x => x.QualityChecks)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }

    public async Task<IReadOnlyList<ProductionOrder>> GetByStatusAsync(ProductionOrderStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(x => x.Status == status && !x.IsDeleted)
            .OrderBy(x => x.PlannedStartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ProductionOrder>> GetOpenOrdersAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(x =>
                !x.IsDeleted &&
                x.Status != ProductionOrderStatus.Completed &&
                x.Status != ProductionOrderStatus.Closed &&
                x.Status != ProductionOrderStatus.Cancelled)
            .OrderBy(x => x.PlannedStartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ProductionOrder>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(x => x.ProductId == productId && !x.IsDeleted)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ProductionOrder>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(x =>
                !x.IsDeleted &&
                x.PlannedStartDate >= startDate &&
                x.PlannedStartDate <= endDate)
            .OrderBy(x => x.PlannedStartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ProductionOrderNumberExistsAsync(string productionOrderNumber, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(
            x => x.ProductionOrderNumber == productionOrderNumber &&
                 !x.IsDeleted &&
                 (!excludeId.HasValue || x.Id != excludeId.Value),
            cancellationToken);
    }
}
