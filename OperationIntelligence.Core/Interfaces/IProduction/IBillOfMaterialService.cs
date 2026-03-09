namespace OperationIntelligence.Core;

public interface IBillOfMaterialService
{
    Task<BillOfMaterialResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResponse<BillOfMaterialSummaryResponse>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<BillOfMaterialResponse> CreateAsync(CreateBillOfMaterialRequest request, string? createdBy = null, CancellationToken cancellationToken = default);
    Task<BillOfMaterialItemResponse> AddItemAsync(CreateBillOfMaterialItemRequest request, string? createdBy = null, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, string? deletedBy = null, CancellationToken cancellationToken = default);
}
