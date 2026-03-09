namespace OperationIntelligence.Core;

public interface IShipmentAddressService
{
    Task<ShipmentAddressResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ShipmentAddressResponse>> SearchAsync(
        string? search = null,
        string? country = null,
        string? city = null,
        int take = 25,
        CancellationToken cancellationToken = default);

    Task<ShipmentAddressResponse> CreateAsync(
        CreateShipmentAddressRequest request,
        string? currentUser = null,
        CancellationToken cancellationToken = default);

    Task<ShipmentAddressResponse> UpdateAsync(
        Guid id,
        UpdateShipmentAddressRequest request,
        string? currentUser = null,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        Guid id,
        string? currentUser = null,
        CancellationToken cancellationToken = default);
}
