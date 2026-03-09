using OperationIntelligence.Core.Models.Scheduling.Requests.Capacity;
using OperationIntelligence.Core.Models.Scheduling.Responses.Capacity;

namespace OperationIntelligence.Core;

public interface ICapacityService
{
    Task<CapacityReservationResponse> CreateReservationAsync(
        CreateCapacityReservationRequest request,
        CancellationToken cancellationToken = default);

    Task<CapacityReservationResponse> UpdateReservationAsync(
        Guid id,
        UpdateCapacityReservationRequest request,
        CancellationToken cancellationToken = default);

    Task<CapacityReservationResponse?> GetReservationByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteReservationAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<PagedResponse<ResourceCapacitySnapshotResponse>> GetUtilizationAsync(
        GetCapacityUtilizationRequest request,
        CancellationToken cancellationToken = default);
}
