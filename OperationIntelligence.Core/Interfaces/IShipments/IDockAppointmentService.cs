using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public interface IDockAppointmentService
{
    Task<PagedResponse<DockAppointmentResponse>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? search = null,
        DockAppointmentStatus? status = null,
        Guid? warehouseId = null,
        Guid? carrierId = null,
        DateTime? scheduledStartFromUtc = null,
        DateTime? scheduledStartToUtc = null,
        CancellationToken cancellationToken = default);

    Task<DockAppointmentResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<DockAppointmentResponse> CreateAsync(
        CreateDockAppointmentRequest request,
        string? currentUser = null,
        CancellationToken cancellationToken = default);

    Task<DockAppointmentResponse> UpdateAsync(
        Guid id,
        UpdateDockAppointmentRequest request,
        string? currentUser = null,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        Guid id,
        string? currentUser = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DockAppointmentResponse>> GetUpcomingAsync(
        Guid? warehouseId = null,
        DateTime? fromUtc = null,
        CancellationToken cancellationToken = default);
}
