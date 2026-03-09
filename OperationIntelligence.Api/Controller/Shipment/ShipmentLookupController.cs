using Microsoft.AspNetCore.Mvc;
using OperationIntelligence.Core;

namespace OperationIntelligence.Api.Controllers.Shipment;

[Route("api/shipment-lookups")]
public class ShipmentLookupController : BaseApiController
{
    private readonly IShipmentLookupService _shipmentLookupService;

    public ShipmentLookupController(IShipmentLookupService shipmentLookupService)
    {
        _shipmentLookupService = shipmentLookupService;
    }

    [HttpGet("carriers")]
    public async Task<IActionResult> GetActiveCarriers(CancellationToken cancellationToken)
    {
        var result = await _shipmentLookupService.GetActiveCarriersAsync(cancellationToken);
        return OkResponse(result);
    }

    [HttpGet("carriers/{carrierId:guid}/services")]
    public async Task<IActionResult> GetActiveCarrierServices(Guid carrierId, CancellationToken cancellationToken)
    {
        var result = await _shipmentLookupService.GetActiveCarrierServicesAsync(carrierId, cancellationToken);
        return OkResponse(result);
    }

    [HttpGet("warehouses")]
    public async Task<IActionResult> GetShipmentWarehouses(CancellationToken cancellationToken)
    {
        var result = await _shipmentLookupService.GetShipmentWarehousesAsync(cancellationToken);
        return OkResponse(result);
    }
}