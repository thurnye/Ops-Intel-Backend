using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class ProductionDowntimeRepository : BaseRepository<ProductionDowntime>, IProductionDowntimeRepository
{
    public ProductionDowntimeRepository(OperationIntelligenceDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<ProductionDowntime>> GetByProductionExecutionIdAsync(Guid productionExecutionId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(x => x.ProductionExecutionId == productionExecutionId && !x.IsDeleted)
            .OrderByDescending(x => x.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ProductionDowntime>> GetByReasonAsync(DowntimeReasonType reason, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(x => x.Reason == reason && !x.IsDeleted)
            .OrderByDescending(x => x.StartTime)
            .ToListAsync(cancellationToken);
    }
}
