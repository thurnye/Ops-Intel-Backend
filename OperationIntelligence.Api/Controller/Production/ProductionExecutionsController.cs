using Microsoft.AspNetCore.Mvc;
using OperationIntelligence.Api.Controllers;
using OperationIntelligence.Core;
using OperationIntelligence.Core.Models.Production.Requests;

namespace OperationIntelligence.Api.Controllers.Production;

[ApiController]
[Route("api/production/executions")]
public class ProductionExecutionsController : BaseApiController
{
    private readonly IProductionExecutionService _productionExecutionService;

    public ProductionExecutionsController(IProductionExecutionService productionExecutionService)
    {
        _productionExecutionService = productionExecutionService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var result = await _productionExecutionService.GetPagedAsync(pageNumber, pageSize, cancellationToken);
        return PagedOkResponse(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _productionExecutionService.GetByIdAsync(id, cancellationToken);
        if (result is null)
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, "Production execution not found.");

        return OkResponse(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductionExecutionRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _productionExecutionService.CreateAsync(request, User?.Identity?.Name, cancellationToken);
        return CreatedResponse(nameof(GetById), new { id = result.Id }, result);
    }
}
