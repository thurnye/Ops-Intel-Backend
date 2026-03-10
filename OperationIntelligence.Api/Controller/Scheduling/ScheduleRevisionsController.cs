using Microsoft.AspNetCore.Mvc;
using OperationIntelligence.Api.Controllers;
using OperationIntelligence.Core;
using OperationIntelligence.Core.Models.Scheduling.Requests.Revision;

namespace OperationIntelligence.Api.Controllers.Scheduling;

[Route("api/scheduling/revisions")]
public class ScheduleRevisionsController : BaseApiController
{
    private readonly IScheduleRevisionService _scheduleRevisionService;

    public ScheduleRevisionsController(IScheduleRevisionService scheduleRevisionService)
    {
        _scheduleRevisionService = scheduleRevisionService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateRevision([FromBody] CreateScheduleRevisionRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _scheduleRevisionService.CreateRevisionAsync(request, cancellationToken);
            return CreatedResponse(result);
        }
        catch (InvalidOperationException ex)
        {
            return ErrorResponse(StatusCodes.Status409Conflict, ErrorCode.CONFLICT_ERROR, ex.Message);
        }
    }

    [HttpPost("reschedule-history")]
    public async Task<IActionResult> CreateRescheduleHistory([FromBody] CreateScheduleRescheduleHistoryRequest request, CancellationToken cancellationToken)
    {
        var result = await _scheduleRevisionService.CreateRescheduleHistoryAsync(request, cancellationToken);
        return CreatedResponse(result);
    }

    [HttpPost("status-history")]
    public async Task<IActionResult> CreateStatusHistory([FromBody] CreateScheduleStatusHistoryRequest request, CancellationToken cancellationToken)
    {
        var result = await _scheduleRevisionService.CreateStatusHistoryAsync(request, cancellationToken);
        return CreatedResponse(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetRevisionById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _scheduleRevisionService.GetRevisionByIdAsync(id, cancellationToken);
        if (result is null)
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, SchedulingErrorMessages.ScheduleRevisionNotFound);

        return OkResponse(result);
    }

    [HttpGet("reschedule-history/{id:guid}")]
    public async Task<IActionResult> GetRescheduleHistoryById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _scheduleRevisionService.GetRescheduleHistoryByIdAsync(id, cancellationToken);
        if (result is null)
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, SchedulingErrorMessages.ScheduleRescheduleHistoryNotFound);

        return OkResponse(result);
    }

    [HttpGet("status-history/{id:guid}")]
    public async Task<IActionResult> GetStatusHistoryById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _scheduleRevisionService.GetStatusHistoryByIdAsync(id, cancellationToken);
        if (result is null)
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, SchedulingErrorMessages.ScheduleStatusHistoryNotFound);

        return OkResponse(result);
    }
}
