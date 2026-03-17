using Microsoft.AspNetCore.Mvc;
using OperationIntelligence.Api.Controllers;
using OperationIntelligence.Core;
using OperationIntelligence.Core.Models.Scheduling.Requests.SchedulePlan;

namespace OperationIntelligence.Api.Controllers.Scheduling;

[Route("api/scheduling/plans")]
public class SchedulePlansController : BaseApiController
{
    private readonly ISchedulePlanService _schedulePlanService;

    public SchedulePlansController(ISchedulePlanService schedulePlanService)
    {
        _schedulePlanService = schedulePlanService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSchedulePlanRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _schedulePlanService.CreateAsync(request, cancellationToken);
            return CreatedResponse(nameof(GetById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            return ErrorResponse(StatusCodes.Status409Conflict, ErrorCode.CONFLICT_ERROR, ex.Message);
        }
    }

    [HttpPost("bulk")]
    public async Task<IActionResult> CreateBulk([FromBody] BulkCreateRequest<CreateSchedulePlanRequest> request, CancellationToken cancellationToken)
    {
        var result = await _schedulePlanService.CreateBulkAsync(request, cancellationToken);
        return OkResponse(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSchedulePlanRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _schedulePlanService.UpdateAsync(id, request, cancellationToken);
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

    [HttpPost("{id:guid}/publish")]
    public async Task<IActionResult> Publish(Guid id, [FromBody] PublishSchedulePlanRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _schedulePlanService.PublishAsync(id, request, cancellationToken);
            return OkResponse(result);
        }
        catch (KeyNotFoundException ex)
        {
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, ex.Message);
        }
    }

    [HttpPost("{id:guid}/clone")]
    public async Task<IActionResult> Clone(Guid id, [FromBody] CloneSchedulePlanRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _schedulePlanService.CloneAsync(id, request, cancellationToken);
            return CreatedResponse(nameof(GetById), new { id = result.Id }, result);
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

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _schedulePlanService.GetByIdAsync(id, cancellationToken);
        if (result is null)
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, SchedulingErrorMessages.SchedulePlanNotFound);

        return OkResponse(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetSchedulePlansRequest request, CancellationToken cancellationToken)
    {
        var result = await _schedulePlanService.GetAllAsync(request, cancellationToken);
        return PagedOkResponse(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _schedulePlanService.DeleteAsync(id, cancellationToken);
        if (!deleted)
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, SchedulingErrorMessages.SchedulePlanNotFound);

        return OkResponse(new { Message = SchedulingErrorMessages.SchedulePlanDeletedSuccessfully });
    }
}
