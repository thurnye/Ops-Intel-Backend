namespace OperationIntelligence.Core;

public interface IShipmentService
{
    Task<PagedResponse<ShipmentListItemResponse>> GetPagedAsync(
        ShipmentFilterRequest request,
        CancellationToken cancellationToken = default);

    Task<ShipmentResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<ShipmentResponse> CreateAsync(
        CreateShipmentRequest request,
        string? currentUser = null,
        CancellationToken cancellationToken = default);

    Task<ShipmentResponse> UpdateAsync(
        Guid id,
        UpdateShipmentRequest request,
        string? currentUser = null,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        Guid id,
        string? currentUser = null,
        CancellationToken cancellationToken = default);

    Task<ShipmentItemResponse> AddItemAsync(
        Guid shipmentId,
        AddShipmentItemRequest request,
        string? currentUser = null,
        CancellationToken cancellationToken = default);

    Task<ShipmentItemResponse> UpdateItemAsync(
        Guid shipmentId,
        Guid shipmentItemId,
        UpdateShipmentItemRequest request,
        string? currentUser = null,
        CancellationToken cancellationToken = default);

    Task<bool> RemoveItemAsync(
        Guid shipmentId,
        Guid shipmentItemId,
        string? currentUser = null,
        CancellationToken cancellationToken = default);

    Task<ShipmentPackageResponse> AddPackageAsync(
        Guid shipmentId,
        AddShipmentPackageRequest request,
        string? currentUser = null,
        CancellationToken cancellationToken = default);

    Task<ShipmentPackageResponse> UpdatePackageAsync(
        Guid shipmentId,
        Guid shipmentPackageId,
        UpdateShipmentPackageRequest request,
        string? currentUser = null,
        CancellationToken cancellationToken = default);

    Task<bool> RemovePackageAsync(
        Guid shipmentId,
        Guid shipmentPackageId,
        string? currentUser = null,
        CancellationToken cancellationToken = default);

    Task<ShipmentPackageItemResponse> AddPackageItemAsync(
        Guid shipmentId,
        Guid shipmentPackageId,
        AddShipmentPackageItemRequest request,
        string? currentUser = null,
        CancellationToken cancellationToken = default);

    Task<bool> RemovePackageItemAsync(
        Guid shipmentId,
        Guid shipmentPackageId,
        Guid shipmentPackageItemId,
        string? currentUser = null,
        CancellationToken cancellationToken = default);

    Task<ShipmentTrackingEventResponse> AddTrackingEventAsync(
        Guid shipmentId,
        AddShipmentTrackingEventRequest request,
        string? currentUser = null,
        CancellationToken cancellationToken = default);

    Task<ShipmentDocumentResponse> AddDocumentAsync(
        Guid shipmentId,
        AddShipmentDocumentRequest request,
        string? currentUser = null,
        CancellationToken cancellationToken = default);

    Task<ShipmentChargeResponse> AddChargeAsync(
        Guid shipmentId,
        AddShipmentChargeRequest request,
        string? currentUser = null,
        CancellationToken cancellationToken = default);

    Task<ShipmentExceptionResponse> AddExceptionAsync(
        Guid shipmentId,
        AddShipmentExceptionRequest request,
        string? currentUser = null,
        CancellationToken cancellationToken = default);

    Task<ShipmentInsuranceResponse> AddInsuranceAsync(
        Guid shipmentId,
        AddShipmentInsuranceRequest request,
        string? currentUser = null,
        CancellationToken cancellationToken = default);

    Task<CustomsDocumentResponse> AddCustomsDocumentAsync(
        Guid shipmentId,
        AddCustomsDocumentRequest request,
        string? currentUser = null,
        CancellationToken cancellationToken = default);

    Task<ShipmentResponse> UpdateStatusAsync(
        Guid shipmentId,
        UpdateShipmentStatusRequest request,
        string? currentUser = null,
        CancellationToken cancellationToken = default);

    Task<ShipmentResponse> AssignToDeliveryRunAsync(
        Guid shipmentId,
        AssignShipmentToDeliveryRunRequest request,
        string? currentUser = null,
        CancellationToken cancellationToken = default);

    Task<ShipmentResponse> UnassignFromDeliveryRunAsync(
        Guid shipmentId,
        string? currentUser = null,
        CancellationToken cancellationToken = default);

    Task<ShipmentResponse> AssignToDockAppointmentAsync(
        Guid shipmentId,
        AssignShipmentToDockAppointmentRequest request,
        string? currentUser = null,
        CancellationToken cancellationToken = default);

    Task<ShipmentResponse> UnassignFromDockAppointmentAsync(
        Guid shipmentId,
        string? currentUser = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ShipmentItemResponse>> GetItemsAsync(
        Guid shipmentId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ShipmentPackageResponse>> GetPackagesAsync(
        Guid shipmentId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ShipmentTrackingEventResponse>> GetTrackingEventsAsync(
        Guid shipmentId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ShipmentDocumentResponse>> GetDocumentsAsync(
        Guid shipmentId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ShipmentChargeResponse>> GetChargesAsync(
        Guid shipmentId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ShipmentExceptionResponse>> GetExceptionsAsync(
        Guid shipmentId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ShipmentInsuranceResponse>> GetInsurancesAsync(
        Guid shipmentId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ShipmentStatusHistoryResponse>> GetStatusHistoryAsync(
        Guid shipmentId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CustomsDocumentResponse>> GetCustomsDocumentsAsync(
        Guid shipmentId,
        CancellationToken cancellationToken = default);

    Task<ShipmentSummaryResponse> GetSummaryAsync(
        ShipmentFilterRequest request,
        CancellationToken cancellationToken = default);
}
