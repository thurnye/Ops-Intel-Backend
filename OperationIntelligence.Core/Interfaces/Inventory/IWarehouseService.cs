namespace OperationIntelligence.Core;

public interface IWarehouseService
{
    Task<WarehouseResponse> CreateAsync(CreateWarehouseRequest request, CancellationToken cancellationToken = default);
    Task<WarehouseResponse?> UpdateAsync(UpdateWarehouseRequest request, CancellationToken cancellationToken = default);
    Task<WarehouseResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<WarehouseResponse>> GetAllAsync(CancellationToken cancellationToken = default);
}
