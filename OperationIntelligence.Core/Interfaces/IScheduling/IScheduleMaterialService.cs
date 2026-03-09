using OperationIntelligence.Core.Models.Scheduling.Requests.Material;
using OperationIntelligence.Core.Models.Scheduling.Responses.Material;

namespace OperationIntelligence.Core;

public interface IScheduleMaterialService
{
    Task<ScheduleMaterialCheckResponse> CreateAsync(
        CreateScheduleMaterialCheckRequest request,
        CancellationToken cancellationToken = default);

    Task<ScheduleMaterialCheckResponse> UpdateAsync(
        Guid id,
        UpdateScheduleMaterialCheckRequest request,
        CancellationToken cancellationToken = default);

    Task<ScheduleMaterialCheckResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
