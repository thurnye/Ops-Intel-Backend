using Microsoft.AspNetCore.Mvc;
using OperationIntelligence.Core;

namespace OperationIntelligence.Api.Controllers.Shipment;

[Route("api/shipment-addresses")]
public class ShipmentAddressesController : BaseApiController
{
    private readonly IShipmentAddressService _shipmentAddressService;

    public ShipmentAddressesController(IShipmentAddressService shipmentAddressService)
    {
        _shipmentAddressService = shipmentAddressService;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _shipmentAddressService.GetByIdAsync(id, cancellationToken);
        if (result == null)
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, "Shipment address not found.");

        return OkResponse(result);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string? search = null,
        [FromQuery] string? country = null,
        [FromQuery] string? city = null,
        [FromQuery] int take = 25,
        CancellationToken cancellationToken = default)
    {
        var result = await _shipmentAddressService.SearchAsync(search, country, city, take, cancellationToken);
        return OkResponse(result);
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary(
        [FromQuery] string? search = null,
        [FromQuery] string? country = null,
        [FromQuery] string? city = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _shipmentAddressService.GetSummaryAsync(search, country, city, cancellationToken);
        return OkResponse(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateShipmentAddressRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _shipmentAddressService.CreateAsync(request, User?.Identity?.Name, cancellationToken);
        return CreatedResponse(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateShipmentAddressRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _shipmentAddressService.UpdateAsync(id, request, User?.Identity?.Name, cancellationToken);
        return OkResponse(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _shipmentAddressService.DeleteAsync(id, User?.Identity?.Name, cancellationToken);
        if (!deleted)
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, "Shipment address not found.");

        return OkResponse(new { Message = "Shipment address deleted successfully." });
    }
}
