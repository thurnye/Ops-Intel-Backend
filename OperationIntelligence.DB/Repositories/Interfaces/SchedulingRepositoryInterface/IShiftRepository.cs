namespace OperationIntelligence.DB;


public interface IShiftRepository
{
    Task<Shift?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Shift?> GetByCodeAsync(Guid warehouseId, string shiftCode, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Shift> Items, int TotalRecords)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Shift>> GetByWarehouseIdAsync(Guid warehouseId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Shift>> GetByWorkCenterIdAsync(Guid workCenterId, CancellationToken cancellationToken = default);

    Task AddAsync(Shift entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(Shift entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(Shift entity, CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(Guid warehouseId, string shiftCode, Guid? excludeId = null, CancellationToken cancellationToken = default);
}
