namespace OperationIntelligence.Core;

public interface IProductionDowntimeService
{
    Task<IReadOnlyList<ProductionDowntimeResponse>> GetByProductionExecutionIdAsync(Guid productionExecutionId, CancellationToken cancellationToken = default);
    Task<ProductionDowntimeResponse> CreateAsync(CreateProductionDowntimeRequest request, string? createdBy = null, CancellationToken cancellationToken = default);
}
