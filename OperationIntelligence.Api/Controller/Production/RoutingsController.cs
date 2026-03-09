using Microsoft.AspNetCore.Mvc;
using OperationIntelligence.Api.Controllers;
using OperationIntelligence.Api.Models;
using OperationIntelligence.Core;
using OperationIntelligence.Core.Models.Production.Requests;

namespace OperationIntelligence.Api.Controllers.Production;

[ApiController]
[Route("api/production/routings")]
public class RoutingsController : BaseApiController
{
    private readonly IRoutingService _routingService;

    public RoutingsController(IRoutingService routingService)
    {
        _routingService = routingService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var result = await _routingService.GetPagedAsync(pageNumber, pageSize, cancellationToken);
        return PagedOkResponse(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _routingService.GetByIdAsync(id, cancellationToken);
        if (result is null)
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, "Routing not found.");

        return OkResponse(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRoutingRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _routingService.CreateAsync(request, User?.Identity?.Name, cancellationToken);
        return CreatedResponse(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPost("{routingId:guid}/steps")]
    public async Task<IActionResult> AddStep(Guid routingId, [FromBody] CreateRoutingStepRequest request, CancellationToken cancellationToken = default)
    {
        request.RoutingId = routingId;
        var result = await _routingService.AddStepAsync(request, User?.Identity?.Name, cancellationToken);
        return CreatedResponse(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        var deleted = await _routingService.DeleteAsync(id, User?.Identity?.Name, cancellationToken);
        if (!deleted)
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, "Routing not found.");

        return OkResponse(new { message = "Routing deleted successfully." });
    }
}
