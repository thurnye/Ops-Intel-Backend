namespace OperationIntelligence.DB;

public interface ISupplierRepository : IBaseRepository<Supplier>
{
    Task<Supplier?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}
