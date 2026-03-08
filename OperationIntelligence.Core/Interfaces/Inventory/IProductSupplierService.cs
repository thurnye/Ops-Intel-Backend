namespace OperationIntelligence.Core;

public interface IProductSupplierService
{
    Task<ProductSupplierResponse> AssignAsync(AssignProductSupplierRequest request, CancellationToken cancellationToken = default);
    Task<ProductSupplierResponse?> UpdateAsync(UpdateProductSupplierRequest request, CancellationToken cancellationToken = default);
    Task<bool> RemoveAsync(Guid productSupplierId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ProductSupplierResponse>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
}
