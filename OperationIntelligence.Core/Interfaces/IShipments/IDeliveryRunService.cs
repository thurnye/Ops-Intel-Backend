using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public interface IDeliveryRunService
{
    Task<PagedResponse<DeliveryRunResponse>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? search = null,
        DeliveryRunStatus? status = null,
        Guid? warehouseId = null,
        DateTime? plannedStartFromUtc = null,
        DateTime? plannedStartToUtc = null,
        CancellationToken cancellationToken = default);

    Task<DeliveryRunResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<DeliveryRunResponse> CreateAsync(
        CreateDeliveryRunRequest request,
        string? currentUser = null,
        CancellationToken cancellationToken = default);

    Task<DeliveryRunResponse> UpdateAsync(
        Guid id,
        UpdateDeliveryRunRequest request,
        string? currentUser = null,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        Guid id,
        string? currentUser = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ShipmentListItemResponse>> GetAssignedShipmentsAsync(
        Guid deliveryRunId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DeliveryRunResponse>> GetActiveRunsAsync(
        Guid? warehouseId = null,
        CancellationToken cancellationToken = default);
}
