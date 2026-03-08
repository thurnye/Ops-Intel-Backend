namespace OperationIntelligence.DB;

public interface IWarehouseRepository : IBaseRepository<Warehouse>
{
    Task<Warehouse?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}
