using Microsoft.AspNetCore.Mvc;
using OperationIntelligence.Api.Controllers;
using OperationIntelligence.Api.Models;
using OperationIntelligence.Core;

namespace OperationIntelligence.Api.Controllers.Production;

[ApiController]
[Route("api/production/routing-steps")]
public class RoutingStepsController : BaseApiController
{
    private readonly IRoutingStepService _routingStepService;

    public RoutingStepsController(IRoutingStepService routingStepService)
    {
        _routingStepService = routingStepService;
    }

    [HttpGet("by-routing/{routingId:guid}")]
    public async Task<IActionResult> GetByRoutingId(Guid routingId, CancellationToken cancellationToken = default)
    {
        var result = await _routingStepService.GetByRoutingIdAsync(routingId, cancellationToken);
        return OkResponse(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _routingStepService.GetByIdAsync(id, cancellationToken);
        if (result is null)
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, "Routing step not found.");

        return OkResponse(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        var deleted = await _routingStepService.DeleteAsync(id, User?.Identity?.Name, cancellationToken);
        if (!deleted)
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, "Routing step not found.");

        return OkResponse(new { message = "Routing step deleted successfully." });
    }
}
