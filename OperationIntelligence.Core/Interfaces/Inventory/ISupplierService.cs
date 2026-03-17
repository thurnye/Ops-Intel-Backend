namespace OperationIntelligence.Core;

public interface ISupplierService
{
    Task<SupplierResponse> CreateAsync(CreateSupplierRequest request, CancellationToken cancellationToken = default);
    Task<BulkCreateResponse<SupplierResponse>> CreateBulkAsync(BulkCreateRequest<CreateSupplierRequest> request, CancellationToken cancellationToken = default);
    Task<SupplierResponse?> UpdateAsync(UpdateSupplierRequest request, CancellationToken cancellationToken = default);
    Task<SupplierResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SupplierResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<SupplierMetricsSummaryResponse> GetSummaryAsync(CancellationToken cancellationToken = default);
}
