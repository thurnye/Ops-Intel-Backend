namespace OperationIntelligence.Core;

public interface IProductionExecutionService
{
    Task<ProductionExecutionResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResponse<ProductionExecutionSummaryResponse>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<ProductionExecutionMetricsSummaryResponse> GetSummaryAsync(CancellationToken cancellationToken = default);
    Task<ProductionExecutionResponse> CreateAsync(CreateProductionExecutionRequest request, string? createdBy = null, CancellationToken cancellationToken = default);
}
