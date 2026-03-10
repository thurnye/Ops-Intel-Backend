using Microsoft.AspNetCore.Mvc;
using OperationIntelligence.Api.Controllers;
using OperationIntelligence.Core;
using OperationIntelligence.Core.Models.Scheduling.Requests.Dispatch;

namespace OperationIntelligence.Api.Controllers.Scheduling;

[Route("api/scheduling/dispatch")]
public class DispatchController : BaseApiController
{
    private readonly IDispatchService _dispatchService;

    public DispatchController(IDispatchService dispatchService)
    {
        _dispatchService = dispatchService;
    }

    [HttpPost("queue-items")]
    public async Task<IActionResult> CreateQueueItem([FromBody] CreateDispatchQueueItemRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _dispatchService.CreateQueueItemAsync(request, cancellationToken);
            return CreatedResponse(result);
        }
        catch (InvalidOperationException ex)
        {
            return ErrorResponse(StatusCodes.Status409Conflict, ErrorCode.CONFLICT_ERROR, ex.Message);
        }
    }

    [HttpPost("queue-items/{id:guid}/release")]
    public async Task<IActionResult> Release(Guid id, [FromBody] ReleaseDispatchQueueItemRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _dispatchService.ReleaseAsync(id, request, cancellationToken);
            return OkResponse(result);
        }
        catch (KeyNotFoundException ex)
        {
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, ex.Message);
        }
    }

    [HttpPost("queue-items/{id:guid}/acknowledge")]
    public async Task<IActionResult> Acknowledge(Guid id, [FromBody] AcknowledgeDispatchQueueItemRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _dispatchService.AcknowledgeAsync(id, request, cancellationToken);
            return OkResponse(result);
        }
        catch (KeyNotFoundException ex)
        {
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, ex.Message);
        }
    }

    [HttpPost("resequence")]
    public async Task<IActionResult> Resequence([FromBody] ResequenceDispatchQueueRequest request, CancellationToken cancellationToken)
    {
        var result = await _dispatchService.ResequenceAsync(request, cancellationToken);
        return OkResponse(result);
    }

    [HttpGet("queue-items/{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatchService.GetByIdAsync(id, cancellationToken);
        if (result is null)
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, SchedulingErrorMessages.DispatchQueueItemNotFound);

        return OkResponse(result);
    }

    [HttpDelete("queue-items/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _dispatchService.DeleteAsync(id, cancellationToken);
        if (!deleted)
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, SchedulingErrorMessages.DispatchQueueItemNotFound);

        return OkResponse(new { Message = SchedulingErrorMessages.DispatchQueueItemDeletedSuccessfully });
    }
}
