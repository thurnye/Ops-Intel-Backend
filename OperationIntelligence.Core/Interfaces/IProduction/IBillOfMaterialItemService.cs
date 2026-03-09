namespace OperationIntelligence.Core;

public interface IBillOfMaterialItemService
{
    Task<IReadOnlyList<BillOfMaterialItemResponse>> GetByBillOfMaterialIdAsync(Guid billOfMaterialId, CancellationToken cancellationToken = default);
    Task<BillOfMaterialItemResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, string? deletedBy = null, CancellationToken cancellationToken = default);
}
