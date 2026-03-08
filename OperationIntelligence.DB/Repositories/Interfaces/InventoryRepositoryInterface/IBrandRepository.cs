namespace OperationIntelligence.DB;

public interface IBrandRepository : IBaseRepository<Brand>
{
    Task<Brand?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}
