using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class ShipmentLookupService : IShipmentLookupService
{
    private readonly IShipmentLookupRepository _lookupRepository;

    public ShipmentLookupService(IShipmentLookupRepository lookupRepository)
    {
        _lookupRepository = lookupRepository;
    }

    public async Task<IReadOnlyList<ShipmentLookupResponse>> GetActiveCarriersAsync(CancellationToken cancellationToken = default)
    {
        var items = await _lookupRepository.GetActiveCarriersAsync(cancellationToken);
        return items.Select(x => new ShipmentLookupResponse
        {
            Id = x.Id,
            Code = x.CarrierCode,
            Name = x.Name
        }).ToList();
    }

    public async Task<IReadOnlyList<ShipmentLookupResponse>> GetActiveCarrierServicesAsync(Guid carrierId, CancellationToken cancellationToken = default)
    {
        var items = await _lookupRepository.GetActiveCarrierServicesAsync(carrierId, cancellationToken);
        return items.Select(x => new ShipmentLookupResponse
        {
            Id = x.Id,
            Code = x.ServiceCode,
            Name = x.Name
        }).ToList();
    }

    public async Task<IReadOnlyList<ShipmentLookupResponse>> GetShipmentWarehousesAsync(CancellationToken cancellationToken = default)
    {
        var items = await _lookupRepository.GetShipmentWarehousesAsync(cancellationToken);
        return items.Select(x => new ShipmentLookupResponse
        {
            Id = x.Id,
            Code = x.Code,
            Name = x.Name
        }).ToList();
    }
}