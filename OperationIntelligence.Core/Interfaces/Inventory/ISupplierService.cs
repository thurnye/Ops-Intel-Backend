namespace OperationIntelligence.Core;

public interface ISupplierService
{
    Task<SupplierResponse> CreateAsync(CreateSupplierRequest request, CancellationToken cancellationToken = default);
    Task<SupplierResponse?> UpdateAsync(UpdateSupplierRequest request, CancellationToken cancellationToken = default);
    Task<SupplierResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SupplierResponse>> GetAllAsync(CancellationToken cancellationToken = default);
}
