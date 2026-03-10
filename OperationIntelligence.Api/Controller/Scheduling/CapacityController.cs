using Microsoft.AspNetCore.Mvc;
using OperationIntelligence.Api.Controllers;
using OperationIntelligence.Core;
using OperationIntelligence.Core.Models.Scheduling.Requests.Capacity;

namespace OperationIntelligence.Api.Controllers.Scheduling;

[Route("api/scheduling/capacity")]
public class CapacityController : BaseApiController
{
    private readonly ICapacityService _capacityService;

    public CapacityController(ICapacityService capacityService)
    {
        _capacityService = capacityService;
    }

    [HttpPost("reservations")]
    public async Task<IActionResult> CreateReservation([FromBody] CreateCapacityReservationRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _capacityService.CreateReservationAsync(request, cancellationToken);
            return CreatedResponse(result);
        }
        catch (InvalidOperationException ex)
        {
            return ErrorResponse(StatusCodes.Status409Conflict, ErrorCode.CONFLICT_ERROR, ex.Message);
        }
    }

    [HttpPut("reservations/{id:guid}")]
    public async Task<IActionResult> UpdateReservation(Guid id, [FromBody] UpdateCapacityReservationRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _capacityService.UpdateReservationAsync(id, request, cancellationToken);
            return OkResponse(result);
        }
        catch (KeyNotFoundException ex)
        {
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return ErrorResponse(StatusCodes.Status409Conflict, ErrorCode.CONFLICT_ERROR, ex.Message);
        }
    }

    [HttpGet("reservations/{id:guid}")]
    public async Task<IActionResult> GetReservationById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _capacityService.GetReservationByIdAsync(id, cancellationToken);
        if (result is null)
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, SchedulingErrorMessages.CapacityReservationNotFound);

        return OkResponse(result);
    }

    [HttpDelete("reservations/{id:guid}")]
    public async Task<IActionResult> DeleteReservation(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _capacityService.DeleteReservationAsync(id, cancellationToken);
        if (!deleted)
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, SchedulingErrorMessages.CapacityReservationNotFound);

        return OkResponse(new { Message = SchedulingErrorMessages.CapacityReservationDeletedSuccessfully });
    }

    [HttpGet("utilization")]
    public async Task<IActionResult> GetUtilization([FromQuery] GetCapacityUtilizationRequest request, CancellationToken cancellationToken)
    {
        var result = await _capacityService.GetUtilizationAsync(request, cancellationToken);
        return PagedOkResponse(result);
    }
}
