namespace OperationIntelligence.Core;

public interface IMachineService
{
    Task<MachineResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResponse<MachineResponse>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<MachineResponse> CreateAsync(CreateMachineRequest request, string? createdBy = null, CancellationToken cancellationToken = default);
    Task<MachineResponse?> UpdateAsync(Guid id, UpdateMachineRequest request, string? updatedBy = null, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, string? deletedBy = null, CancellationToken cancellationToken = default);
}
