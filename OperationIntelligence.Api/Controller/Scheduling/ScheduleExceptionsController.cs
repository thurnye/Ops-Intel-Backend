using Microsoft.AspNetCore.Mvc;
using OperationIntelligence.Api.Controllers;
using OperationIntelligence.Api.Models;
using OperationIntelligence.Core;
using OperationIntelligence.Core.Models.Scheduling.Requests.Exception;

namespace OperationIntelligence.Api.Controllers.Scheduling;

[Route("api/scheduling/exceptions")]
public class ScheduleExceptionsController : BaseApiController
{
    private readonly IScheduleExceptionService _scheduleExceptionService;

    public ScheduleExceptionsController(IScheduleExceptionService scheduleExceptionService)
    {
        _scheduleExceptionService = scheduleExceptionService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateScheduleExceptionRequest request, CancellationToken cancellationToken)
    {
        var result = await _scheduleExceptionService.CreateAsync(request, cancellationToken);
        return CreatedResponse(result);
    }

    [HttpPost("{id:guid}/resolve")]
    public async Task<IActionResult> Resolve(Guid id, [FromBody] ResolveScheduleExceptionRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _scheduleExceptionService.ResolveAsync(id, request, cancellationToken);
            return OkResponse(result);
        }
        catch (KeyNotFoundException ex)
        {
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, ex.Message);
        }
    }

    [HttpPut("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateScheduleExceptionStatusRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _scheduleExceptionService.UpdateStatusAsync(id, request, cancellationToken);
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
        var result = await _scheduleExceptionService.GetByIdAsync(id, cancellationToken);
        if (result is null)
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, SchedulingErrorMessages.ScheduleExceptionNotFound);

        return OkResponse(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetScheduleExceptionsRequest request, CancellationToken cancellationToken)
    {
        var result = await _scheduleExceptionService.GetAllAsync(request, cancellationToken);
        return PagedOkResponse(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _scheduleExceptionService.DeleteAsync(id, cancellationToken);
        if (!deleted)
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, SchedulingErrorMessages.ScheduleExceptionNotFound);

        return OkResponse(new { Message = SchedulingErrorMessages.ScheduleExceptionDeletedSuccessfully });
    }
}
