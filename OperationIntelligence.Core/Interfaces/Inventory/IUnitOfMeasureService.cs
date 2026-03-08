namespace OperationIntelligence.Core;

public interface IUnitOfMeasureService
{
    Task<UnitOfMeasureResponse> CreateAsync(CreateUnitOfMeasureRequest request, CancellationToken cancellationToken = default);
    Task<UnitOfMeasureResponse?> UpdateAsync(UpdateUnitOfMeasureRequest request, CancellationToken cancellationToken = default);
    Task<UnitOfMeasureResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UnitOfMeasureResponse>> GetAllAsync(CancellationToken cancellationToken = default);
}
