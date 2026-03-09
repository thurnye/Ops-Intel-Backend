namespace OperationIntelligence.DB;

public interface IBillOfMaterialRepository : IBaseRepository<BillOfMaterial>
{
    Task<BillOfMaterial?> GetWithItemsAsync(Guid id, CancellationToken cancellationToken = default);

    Task<BillOfMaterial?> GetDefaultByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<BillOfMaterial>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);

    Task<BillOfMaterial?> GetActiveVersionAsync(Guid productId, int version, CancellationToken cancellationToken = default);

    Task<bool> BomCodeExistsAsync(string bomCode, Guid? excludeId = null, CancellationToken cancellationToken = default);
}
