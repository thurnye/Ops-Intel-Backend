using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class WarehouseRepository : BaseRepository<Warehouse>, IWarehouseRepository
{
    public WarehouseRepository(OperationIntelligenceDbContext context) : base(context)
    {
    }

    public async Task<Warehouse?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
    }
}
