namespace OperationIntelligence.DB;

public interface IBillOfMaterialItemRepository : IBaseRepository<BillOfMaterialItem>
{
    Task<IReadOnlyList<BillOfMaterialItem>> GetByBillOfMaterialIdAsync(Guid billOfMaterialId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<BillOfMaterialItem>> GetByMaterialProductIdAsync(Guid materialProductId, CancellationToken cancellationToken = default);

    Task<BillOfMaterialItem?> GetByBillOfMaterialAndSequenceAsync(Guid billOfMaterialId, int sequence, CancellationToken cancellationToken = default);
}
