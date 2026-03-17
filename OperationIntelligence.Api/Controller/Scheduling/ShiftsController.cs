using Microsoft.AspNetCore.Mvc;
using OperationIntelligence.Api.Controllers;
using OperationIntelligence.Core;
using OperationIntelligence.Core.Models.Scheduling.Requests.Shift;

namespace OperationIntelligence.Api.Controllers.Scheduling;

[Route("api/scheduling/shifts")]
public class ShiftsController : BaseApiController
{
    private readonly IShiftService _shiftService;

    public ShiftsController(IShiftService shiftService)
    {
        _shiftService = shiftService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateShiftRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _shiftService.CreateAsync(request, cancellationToken);
            return CreatedResponse(nameof(GetById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            return ErrorResponse(StatusCodes.Status409Conflict, ErrorCode.CONFLICT_ERROR, ex.Message);
        }
    }

    [HttpPost("bulk")]
    public async Task<IActionResult> CreateBulk([FromBody] BulkCreateRequest<CreateShiftRequest> request, CancellationToken cancellationToken)
    {
        var result = await _shiftService.CreateBulkAsync(request, cancellationToken);
        return OkResponse(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateShiftRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _shiftService.UpdateAsync(id, request, cancellationToken);
            return OkResponse(result);
        }
        catch (KeyNotFoundException ex)
        {
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, ex.Message);
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _shiftService.GetByIdAsync(id, cancellationToken);
        if (result is null)
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, SchedulingErrorMessages.ShiftNotFound);

        return OkResponse(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetShiftsRequest request, CancellationToken cancellationToken)
    {
        var result = await _shiftService.GetAllAsync(request, cancellationToken);
        return PagedOkResponse(result);
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary(CancellationToken cancellationToken)
    {
        var result = await _shiftService.GetSummaryAsync(cancellationToken);
        return OkResponse(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _shiftService.DeleteAsync(id, cancellationToken);
        if (!deleted)
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, SchedulingErrorMessages.ShiftNotFound);

        return OkResponse(new { Message = SchedulingErrorMessages.ShiftDeletedSuccessfully });
    }
}
