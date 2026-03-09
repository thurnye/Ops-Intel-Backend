namespace OperationIntelligence.Core;

public interface IReturnShipmentService
{
    Task<PagedResponse<ReturnShipmentResponse>> GetPagedAsync(
        ReturnShipmentFilterRequest request,
        CancellationToken cancellationToken = default);

    Task<ReturnShipmentResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<ReturnShipmentResponse> CreateAsync(
        CreateReturnShipmentRequest request,
        string? currentUser = null,
        CancellationToken cancellationToken = default);

    Task<ReturnShipmentResponse> UpdateAsync(
        Guid id,
        UpdateReturnShipmentRequest request,
        string? currentUser = null,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        Guid id,
        string? currentUser = null,
        CancellationToken cancellationToken = default);

    Task<ReturnShipmentItemResponse> AddItemAsync(
        Guid returnShipmentId,
        AddReturnShipmentItemRequest request,
        string? currentUser = null,
        CancellationToken cancellationToken = default);

    Task<bool> RemoveItemAsync(
        Guid returnShipmentId,
        Guid returnShipmentItemId,
        string? currentUser = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ReturnShipmentItemResponse>> GetItemsAsync(
        Guid returnShipmentId,
        CancellationToken cancellationToken = default);
}
