namespace OperationIntelligence.DB;

public interface IWorkCenterRepository : IBaseRepository<WorkCenter>
{
    Task<WorkCenter?> GetWithMachinesAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<WorkCenter>> GetActiveAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<WorkCenter>> GetByWarehouseIdAsync(Guid warehouseId, CancellationToken cancellationToken = default);

    Task<bool> CodeExistsAsync(string code, Guid? excludeId = null, CancellationToken cancellationToken = default);
}
