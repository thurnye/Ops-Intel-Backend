using Microsoft.AspNetCore.Mvc;
using OperationIntelligence.Api.Controllers;
using OperationIntelligence.Core;
using OperationIntelligence.Core.Models.Scheduling.Requests.ScheduleJob;

namespace OperationIntelligence.Api.Controllers.Scheduling;

[Route("api/scheduling/jobs")]
public class ScheduleJobsController : BaseApiController
{
    private readonly IScheduleJobService _scheduleJobService;

    public ScheduleJobsController(IScheduleJobService scheduleJobService)
    {
        _scheduleJobService = scheduleJobService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateScheduleJobRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _scheduleJobService.CreateAsync(request, cancellationToken);
            return CreatedResponse(nameof(GetById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            return ErrorResponse(StatusCodes.Status409Conflict, ErrorCode.CONFLICT_ERROR, ex.Message);
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateScheduleJobRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _scheduleJobService.UpdateAsync(id, request, cancellationToken);
            return OkResponse(result);
        }
        catch (KeyNotFoundException ex)
        {
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, ex.Message);
        }
    }

    [HttpPost("{id:guid}/release")]
    public async Task<IActionResult> Release(Guid id, [FromBody] ReleaseScheduleJobRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _scheduleJobService.ReleaseAsync(id, request, cancellationToken);
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

    [HttpPost("{id:guid}/pause")]
    public async Task<IActionResult> Pause(Guid id, [FromBody] PauseScheduleJobRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _scheduleJobService.PauseAsync(id, request, cancellationToken);
            return OkResponse(result);
        }
        catch (KeyNotFoundException ex)
        {
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, ex.Message);
        }
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, [FromBody] CancelScheduleJobRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _scheduleJobService.CancelAsync(id, request, cancellationToken);
            return OkResponse(result);
        }
        catch (KeyNotFoundException ex)
        {
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, ex.Message);
        }
    }

    [HttpPost("{id:guid}/reschedule")]
    public async Task<IActionResult> Reschedule(Guid id, [FromBody] RescheduleJobRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _scheduleJobService.RescheduleAsync(id, request, cancellationToken);
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
        var result = await _scheduleJobService.GetByIdAsync(id, cancellationToken);
        if (result is null)
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, SchedulingErrorMessages.ScheduleJobNotFound);

        return OkResponse(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetScheduleJobsRequest request, CancellationToken cancellationToken)
    {
        var result = await _scheduleJobService.GetAllAsync(request, cancellationToken);
        return PagedOkResponse(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _scheduleJobService.DeleteAsync(id, cancellationToken);
        if (!deleted)
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, SchedulingErrorMessages.ScheduleJobNotFound);

        return OkResponse(new { Message = SchedulingErrorMessages.ScheduleJobDeletedSuccessfully });
    }
}
