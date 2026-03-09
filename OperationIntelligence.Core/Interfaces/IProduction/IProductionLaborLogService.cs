namespace OperationIntelligence.Core;

public interface IProductionLaborLogService
{
    Task<IReadOnlyList<ProductionLaborLogResponse>> GetByProductionExecutionIdAsync(Guid productionExecutionId, CancellationToken cancellationToken = default);
    Task<ProductionLaborLogResponse> CreateAsync(CreateProductionLaborLogRequest request, string? createdBy = null, CancellationToken cancellationToken = default);
}
