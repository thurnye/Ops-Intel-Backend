using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class BrandRepository : BaseRepository<Brand>, IBrandRepository
{
    public BrandRepository(OperationIntelligenceDbContext context) : base(context)
    {
    }

    public async Task<Brand?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
    }
}
