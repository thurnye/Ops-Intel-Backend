using Microsoft.AspNetCore.Mvc;
using OperationIntelligence.Api.Models;
using OperationIntelligence.Core;

namespace OperationIntelligence.Api.Controllers.Shipment;

[Route("api/shipment-carriers")]
public class CarriersController : BaseApiController
{
    private readonly ICarrierService _carrierService;

    public CarriersController(ICarrierService carrierService)
    {
        _carrierService = carrierService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 25,
        [FromQuery] string? search = null,
        [FromQuery] bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _carrierService.GetPagedAsync(pageNumber, pageSize, search, isActive, cancellationToken);
        return PagedOkResponse(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _carrierService.GetByIdAsync(id, cancellationToken);
        if (result == null)
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, "Carrier not found.");

        return OkResponse(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateCarrierRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _carrierService.CreateAsync(request, User?.Identity?.Name, cancellationToken);
        return CreatedResponse(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateCarrierRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _carrierService.UpdateAsync(id, request, User?.Identity?.Name, cancellationToken);
        return OkResponse(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _carrierService.DeleteAsync(id, User?.Identity?.Name, cancellationToken);
        if (!deleted)
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, "Carrier not found.");

        return OkResponse(new { Message = "Carrier deleted successfully." });
    }

    [HttpGet("{carrierId:guid}/services")]
    public async Task<IActionResult> GetServices(
        Guid carrierId,
        [FromQuery] bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _carrierService.GetServicesByCarrierIdAsync(carrierId, isActive, cancellationToken);
        return OkResponse(result);
    }

    [HttpPost("services")]
    public async Task<IActionResult> CreateService(
        [FromBody] CreateCarrierServiceRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _carrierService.CreateServiceAsync(request, User?.Identity?.Name, cancellationToken);
        return CreatedResponse(result);
    }

    [HttpPut("services/{id:guid}")]
    public async Task<IActionResult> UpdateService(
        Guid id,
        [FromBody] UpdateCarrierServiceRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _carrierService.UpdateServiceAsync(id, request, User?.Identity?.Name, cancellationToken);
        return OkResponse(result);
    }

    [HttpDelete("services/{id:guid}")]
    public async Task<IActionResult> DeleteService(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _carrierService.DeleteServiceAsync(id, User?.Identity?.Name, cancellationToken);
        if (!deleted)
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, "Carrier service not found.");

        return OkResponse(new { Message = "Carrier service deleted successfully." });
    }
}