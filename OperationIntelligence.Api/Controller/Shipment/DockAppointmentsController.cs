using Microsoft.AspNetCore.Mvc;
using OperationIntelligence.Api.Models;
using OperationIntelligence.Core;
using OperationIntelligence.DB;

namespace OperationIntelligence.Api.Controllers.Shipment;

[Route("api/dock-appointments")]
public class DockAppointmentsController : BaseApiController
{
    private readonly IDockAppointmentService _dockAppointmentService;

    public DockAppointmentsController(IDockAppointmentService dockAppointmentService)
    {
        _dockAppointmentService = dockAppointmentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 25,
        [FromQuery] string? search = null,
        [FromQuery] DockAppointmentStatus? status = null,
        [FromQuery] Guid? warehouseId = null,
        [FromQuery] Guid? carrierId = null,
        [FromQuery] DateTime? scheduledStartFromUtc = null,
        [FromQuery] DateTime? scheduledStartToUtc = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _dockAppointmentService.GetPagedAsync(
            pageNumber,
            pageSize,
            search,
            status,
            warehouseId,
            carrierId,
            scheduledStartFromUtc,
            scheduledStartToUtc,
            cancellationToken);

        return PagedOkResponse(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dockAppointmentService.GetByIdAsync(id, cancellationToken);
        if (result == null)
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, "Dock appointment not found.");

        return OkResponse(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateDockAppointmentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _dockAppointmentService.CreateAsync(request, User?.Identity?.Name, cancellationToken);
        return CreatedResponse(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateDockAppointmentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _dockAppointmentService.UpdateAsync(id, request, User?.Identity?.Name, cancellationToken);
        return OkResponse(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _dockAppointmentService.DeleteAsync(id, User?.Identity?.Name, cancellationToken);
        if (!deleted)
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, "Dock appointment not found.");

        return OkResponse(new { Message = "Dock appointment deleted successfully." });
    }

    [HttpGet("upcoming")]
    public async Task<IActionResult> GetUpcoming(
        [FromQuery] Guid? warehouseId = null,
        [FromQuery] DateTime? fromUtc = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _dockAppointmentService.GetUpcomingAsync(warehouseId, fromUtc, cancellationToken);
        return OkResponse(result);
    }
}