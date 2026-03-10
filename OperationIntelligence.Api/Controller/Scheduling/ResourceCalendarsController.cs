using Microsoft.AspNetCore.Mvc;
using OperationIntelligence.Api.Controllers;
using OperationIntelligence.Core;
using OperationIntelligence.Core.Models.Scheduling.Requests.ResourceCalendar;

namespace OperationIntelligence.Api.Controllers.Scheduling;

[Route("api/scheduling/resource-calendars")]
public class ResourceCalendarsController : BaseApiController
{
    private readonly IResourceCalendarService _resourceCalendarService;

    public ResourceCalendarsController(IResourceCalendarService resourceCalendarService)
    {
        _resourceCalendarService = resourceCalendarService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateResourceCalendarRequest request, CancellationToken cancellationToken)
    {
        var result = await _resourceCalendarService.CreateAsync(request, cancellationToken);
        return CreatedResponse(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateResourceCalendarRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _resourceCalendarService.UpdateAsync(id, request, cancellationToken);
            return OkResponse(result);
        }
        catch (KeyNotFoundException ex)
        {
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, ex.Message);
        }
    }

    [HttpPost("exceptions")]
    public async Task<IActionResult> AddException([FromBody] CreateResourceCalendarExceptionRequest request, CancellationToken cancellationToken)
    {
        var result = await _resourceCalendarService.AddExceptionAsync(request, cancellationToken);
        return CreatedResponse(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _resourceCalendarService.GetByIdAsync(id, cancellationToken);
        if (result is null)
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, SchedulingErrorMessages.ResourceCalendarNotFound);

        return OkResponse(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _resourceCalendarService.DeleteAsync(id, cancellationToken);
        if (!deleted)
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, SchedulingErrorMessages.ResourceCalendarNotFound);

        return OkResponse(new { Message = SchedulingErrorMessages.ResourceCalendarDeletedSuccessfully });
    }

    [HttpDelete("exceptions/{id:guid}")]
    public async Task<IActionResult> DeleteException(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _resourceCalendarService.DeleteExceptionAsync(id, cancellationToken);
        if (!deleted)
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, SchedulingErrorMessages.ResourceCalendarExceptionNotFound);

        return OkResponse(new { Message = SchedulingErrorMessages.ResourceCalendarExceptionDeletedSuccessfully });
    }
}
