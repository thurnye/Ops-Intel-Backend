using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class ProductionMaterialConsumptionRepository : BaseRepository<ProductionMaterialConsumption>, IProductionMaterialConsumptionRepository
{
    public ProductionMaterialConsumptionRepository(OperationIntelligenceDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<ProductionMaterialConsumption>> GetByProductionMaterialIssueIdAsync(Guid productionMaterialIssueId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(x => x.ProductionMaterialIssueId == productionMaterialIssueId && !x.IsDeleted)
            .OrderBy(x => x.ConsumptionDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ProductionMaterialConsumption>> GetByProductionExecutionIdAsync(Guid productionExecutionId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(x => x.ProductionExecutionId == productionExecutionId && !x.IsDeleted)
            .OrderBy(x => x.ConsumptionDate)
            .ToListAsync(cancellationToken);
    }
}
