using OperationIntelligence.Core.Models.Scheduling.Requests.Shift;
using OperationIntelligence.Core.Models.Scheduling.Responses.Shift;

namespace OperationIntelligence.Core;

public interface IShiftService
{
    Task<ShiftResponse> CreateAsync(
        CreateShiftRequest request,
        CancellationToken cancellationToken = default);
    Task<BulkCreateResponse<ShiftResponse>> CreateBulkAsync(
        BulkCreateRequest<CreateShiftRequest> request,
        CancellationToken cancellationToken = default);

    Task<ShiftResponse> UpdateAsync(
        Guid id,
        UpdateShiftRequest request,
        CancellationToken cancellationToken = default);

    Task<ShiftResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<PagedResponse<ShiftResponse>> GetAllAsync(
        GetShiftsRequest request,
        CancellationToken cancellationToken = default);

    Task<ShiftMetricsSummaryResponse> GetSummaryAsync(
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
