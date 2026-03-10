using Microsoft.AspNetCore.Mvc;
using OperationIntelligence.Core;

namespace OperationIntelligence.Api.Controllers.Shipment;

[Route("api/return-shipments")]
public class ReturnShipmentsController : BaseApiController
{
    private readonly IReturnShipmentService _returnShipmentService;

    public ReturnShipmentsController(IReturnShipmentService returnShipmentService)
    {
        _returnShipmentService = returnShipmentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged(
        [FromQuery] ReturnShipmentFilterRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _returnShipmentService.GetPagedAsync(request, cancellationToken);
        return PagedOkResponse(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _returnShipmentService.GetByIdAsync(id, cancellationToken);
        if (result == null)
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, "Return shipment not found.");

        return OkResponse(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateReturnShipmentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _returnShipmentService.CreateAsync(request, User?.Identity?.Name, cancellationToken);
        return CreatedResponse(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateReturnShipmentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _returnShipmentService.UpdateAsync(id, request, User?.Identity?.Name, cancellationToken);
        return OkResponse(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _returnShipmentService.DeleteAsync(id, User?.Identity?.Name, cancellationToken);
        if (!deleted)
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, "Return shipment not found.");

        return OkResponse(new { Message = "Return shipment deleted successfully." });
    }

    [HttpGet("{returnShipmentId:guid}/items")]
    public async Task<IActionResult> GetItems(Guid returnShipmentId, CancellationToken cancellationToken)
    {
        var result = await _returnShipmentService.GetItemsAsync(returnShipmentId, cancellationToken);
        return OkResponse(result);
    }

    [HttpPost("{returnShipmentId:guid}/items")]
    public async Task<IActionResult> AddItem(
        Guid returnShipmentId,
        [FromBody] AddReturnShipmentItemRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _returnShipmentService.AddItemAsync(returnShipmentId, request, User?.Identity?.Name, cancellationToken);
        return CreatedResponse(result);
    }

    [HttpDelete("{returnShipmentId:guid}/items/{returnShipmentItemId:guid}")]
    public async Task<IActionResult> RemoveItem(
        Guid returnShipmentId,
        Guid returnShipmentItemId,
        CancellationToken cancellationToken)
    {
        var deleted = await _returnShipmentService.RemoveItemAsync(
            returnShipmentId,
            returnShipmentItemId,
            User?.Identity?.Name,
            cancellationToken);

        if (!deleted)
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, "Return shipment item not found.");

        return OkResponse(new { Message = "Return shipment item removed successfully." });
    }
}
