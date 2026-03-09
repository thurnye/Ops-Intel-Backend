namespace OperationIntelligence.Core;

public interface ICarrierService
{
    Task<PagedResponse<CarrierResponse>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? search = null,
        bool? isActive = null,
        CancellationToken cancellationToken = default);

    Task<CarrierResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<CarrierResponse> CreateAsync(
        CreateCarrierRequest request,
        string? currentUser = null,
        CancellationToken cancellationToken = default);

    Task<CarrierResponse> UpdateAsync(
        Guid id,
        UpdateCarrierRequest request,
        string? currentUser = null,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        Guid id,
        string? currentUser = null,
        CancellationToken cancellationToken = default);

    Task<CarrierServiceResponse> CreateServiceAsync(
        CreateCarrierServiceRequest request,
        string? currentUser = null,
        CancellationToken cancellationToken = default);

    Task<CarrierServiceResponse> UpdateServiceAsync(
        Guid id,
        UpdateCarrierServiceRequest request,
        string? currentUser = null,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteServiceAsync(
        Guid id,
        string? currentUser = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CarrierServiceResponse>> GetServicesByCarrierIdAsync(
        Guid carrierId,
        bool? isActive = null,
        CancellationToken cancellationToken = default);
}
