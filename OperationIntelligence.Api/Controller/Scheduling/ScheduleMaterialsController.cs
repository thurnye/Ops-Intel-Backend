using Microsoft.AspNetCore.Mvc;
using OperationIntelligence.Api.Controllers;
using OperationIntelligence.Api.Models;
using OperationIntelligence.Core;
using OperationIntelligence.Core.Models.Scheduling.Requests.Material;

namespace OperationIntelligence.Api.Controllers.Scheduling;

[Route("api/scheduling/material-checks")]
public class ScheduleMaterialsController : BaseApiController
{
    private readonly IScheduleMaterialService _scheduleMaterialService;

    public ScheduleMaterialsController(IScheduleMaterialService scheduleMaterialService)
    {
        _scheduleMaterialService = scheduleMaterialService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateScheduleMaterialCheckRequest request, CancellationToken cancellationToken)
    {
        var result = await _scheduleMaterialService.CreateAsync(request, cancellationToken);
        return CreatedResponse(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateScheduleMaterialCheckRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _scheduleMaterialService.UpdateAsync(id, request, cancellationToken);
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
        var result = await _scheduleMaterialService.GetByIdAsync(id, cancellationToken);
        if (result is null)
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, SchedulingErrorMessages.ScheduleMaterialCheckNotFound);

        return OkResponse(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _scheduleMaterialService.DeleteAsync(id, cancellationToken);
        if (!deleted)
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, SchedulingErrorMessages.ScheduleMaterialCheckNotFound);

        return OkResponse(new { Message = SchedulingErrorMessages.ScheduleMaterialCheckDeletedSuccessfully });
    }
}
