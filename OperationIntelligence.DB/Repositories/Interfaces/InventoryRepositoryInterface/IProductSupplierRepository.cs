namespace OperationIntelligence.DB;

public interface IProductSupplierRepository : IBaseRepository<ProductSupplier>
{
    Task<IReadOnlyList<ProductSupplier>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<ProductSupplier?> GetByProductAndSupplierAsync(Guid productId, Guid supplierId, CancellationToken cancellationToken = default);
}
