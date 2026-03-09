namespace OperationIntelligence.Core;

public interface IRoutingService
{
    Task<RoutingResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResponse<RoutingSummaryResponse>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<RoutingResponse> CreateAsync(CreateRoutingRequest request, string? createdBy = null, CancellationToken cancellationToken = default);
    Task<RoutingStepResponse> AddStepAsync(CreateRoutingStepRequest request, string? createdBy = null, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, string? deletedBy = null, CancellationToken cancellationToken = default);
}
