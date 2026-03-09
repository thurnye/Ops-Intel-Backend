namespace OperationIntelligence.Core;

public interface IWorkCenterService
{
    Task<WorkCenterResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResponse<WorkCenterResponse>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<WorkCenterResponse> CreateAsync(CreateWorkCenterRequest request, string? createdBy = null, CancellationToken cancellationToken = default);
    Task<WorkCenterResponse?> UpdateAsync(Guid id, UpdateWorkCenterRequest request, string? updatedBy = null, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, string? deletedBy = null, CancellationToken cancellationToken = default);
}
