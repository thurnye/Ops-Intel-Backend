using Microsoft.EntityFrameworkCore;


namespace OperationIntelligence.DB;

public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
{
    public CategoryRepository(OperationIntelligenceDbContext context) : base(context)
    {
    }

    public async Task<Category?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
    }
}
