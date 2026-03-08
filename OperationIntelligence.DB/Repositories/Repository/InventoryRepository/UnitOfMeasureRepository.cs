using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class UnitOfMeasureRepository : BaseRepository<UnitOfMeasure>, IUnitOfMeasureRepository
{
    public UnitOfMeasureRepository(OperationIntelligenceDbContext context) : base(context)
    {
    }

    public async Task<UnitOfMeasure?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
    }
}
