namespace OperationIntelligence.DB;

public interface IShipmentAddressRepository : IBaseRepository<ShipmentAddress>
{
    Task<IReadOnlyList<ShipmentAddress>> SearchAsync(
        string? search = null,
        string? country = null,
        string? city = null,
        int take = 25,
        CancellationToken cancellationToken = default);
}
