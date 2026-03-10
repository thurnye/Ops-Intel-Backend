using Microsoft.AspNetCore.Mvc;
using OperationIntelligence.Core;
using OperationIntelligence.DB;

namespace OperationIntelligence.Api.Controllers.Shipment;

[Route("api/delivery-runs")]
public class DeliveryRunsController : BaseApiController
{
    private readonly IDeliveryRunService _deliveryRunService;

    public DeliveryRunsController(IDeliveryRunService deliveryRunService)
    {
        _deliveryRunService = deliveryRunService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 25,
        [FromQuery] string? search = null,
        [FromQuery] DeliveryRunStatus? status = null,
        [FromQuery] Guid? warehouseId = null,
        [FromQuery] DateTime? plannedStartFromUtc = null,
        [FromQuery] DateTime? plannedStartToUtc = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _deliveryRunService.GetPagedAsync(
            pageNumber,
            pageSize,
            search,
            status,
            warehouseId,
            plannedStartFromUtc,
            plannedStartToUtc,
            cancellationToken);

        return PagedOkResponse(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _deliveryRunService.GetByIdAsync(id, cancellationToken);
        if (result == null)
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, "Delivery run not found.");

        return OkResponse(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateDeliveryRunRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _deliveryRunService.CreateAsync(request, User?.Identity?.Name, cancellationToken);
        return CreatedResponse(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateDeliveryRunRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _deliveryRunService.UpdateAsync(id, request, User?.Identity?.Name, cancellationToken);
        return OkResponse(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _deliveryRunService.DeleteAsync(id, User?.Identity?.Name, cancellationToken);
        if (!deleted)
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, "Delivery run not found.");

        return OkResponse(new { Message = "Delivery run deleted successfully." });
    }

    [HttpGet("{deliveryRunId:guid}/shipments")]
    public async Task<IActionResult> GetAssignedShipments(Guid deliveryRunId, CancellationToken cancellationToken)
    {
        var result = await _deliveryRunService.GetAssignedShipmentsAsync(deliveryRunId, cancellationToken);
        return OkResponse(result);
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActiveRuns(
        [FromQuery] Guid? warehouseId = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _deliveryRunService.GetActiveRunsAsync(warehouseId, cancellationToken);
        return OkResponse(result);
    }
}
