using Microsoft.AspNetCore.Mvc;
using OperationIntelligence.Api.Controllers;
using OperationIntelligence.Api.Models;
using OperationIntelligence.Core;

namespace OperationIntelligence.Api.Controllers.Inventory;

[ApiController]
[Route("api/inventory/warehouses")]
public class WarehousesController : BaseApiController
{
    private readonly IWarehouseService _warehouseService;

    public WarehousesController(IWarehouseService warehouseService)
    {
        _warehouseService = warehouseService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateWarehouseRequest request, CancellationToken cancellationToken)
    {
        var result = await _warehouseService.CreateAsync(request, cancellationToken);
        return CreatedResponse(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _warehouseService.GetByIdAsync(id, cancellationToken);
        return result == null ? ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, "Resource not found.") : OkResponse(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _warehouseService.GetAllAsync(cancellationToken);
        return OkResponse(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateWarehouseRequest request, CancellationToken cancellationToken)
    {
        if (id != request.Id)
            return ErrorResponse(StatusCodes.Status400BadRequest, ErrorCode.VALIDATION_ERROR, "Route id does not match request id.");

        var result = await _warehouseService.UpdateAsync(request, cancellationToken);
        return result == null ? ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, "Resource not found.") : OkResponse(result);
    }
}
