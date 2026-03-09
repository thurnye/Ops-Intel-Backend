using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class ProductionExecutionRepository : BaseRepository<ProductionExecution>, IProductionExecutionRepository
{
    public ProductionExecutionRepository(OperationIntelligenceDbContext context) : base(context)
    {
    }

    public async Task<ProductionExecution?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(x => x.ProductionOrder)
            .Include(x => x.RoutingStep)
            .Include(x => x.WorkCenter)
            .Include(x => x.Machine)
            .Include(x => x.MaterialConsumptions)
            .Include(x => x.LaborLogs)
            .Include(x => x.Downtimes)
            .Include(x => x.Scraps)
            .Include(x => x.QualityChecks)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }

    public async Task<IReadOnlyList<ProductionExecution>> GetByProductionOrderIdAsync(Guid productionOrderId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(x => x.RoutingStep)
            .Include(x => x.WorkCenter)
            .Include(x => x.Machine)
            .Where(x => x.ProductionOrderId == productionOrderId && !x.IsDeleted)
            .OrderBy(x => x.PlannedStartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ProductionExecution>> GetByStatusAsync(ExecutionStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(x => x.Status == status && !x.IsDeleted)
            .OrderBy(x => x.PlannedStartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ProductionExecution>> GetByWorkCenterIdAsync(Guid workCenterId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(x => x.WorkCenterId == workCenterId && !x.IsDeleted)
            .OrderBy(x => x.PlannedStartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ProductionExecution>> GetActiveExecutionsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(x =>
                !x.IsDeleted &&
                (x.Status == ExecutionStatus.Ready ||
                 x.Status == ExecutionStatus.Running ||
                 x.Status == ExecutionStatus.Paused))
            .OrderBy(x => x.PlannedStartDate)
            .ToListAsync(cancellationToken);
    }
}
