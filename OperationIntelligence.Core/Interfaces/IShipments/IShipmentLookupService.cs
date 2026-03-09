namespace OperationIntelligence.Core;

public interface IShipmentLookupService
{
    Task<IReadOnlyList<ShipmentLookupResponse>> GetActiveCarriersAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ShipmentLookupResponse>> GetActiveCarrierServicesAsync(
        Guid carrierId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ShipmentLookupResponse>> GetShipmentWarehousesAsync(
        CancellationToken cancellationToken = default);
}
