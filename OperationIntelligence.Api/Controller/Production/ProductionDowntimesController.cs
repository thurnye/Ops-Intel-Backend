using Microsoft.AspNetCore.Mvc;
using OperationIntelligence.Api.Controllers;
using OperationIntelligence.Core;
using OperationIntelligence.Core.Models.Production.Requests;

namespace OperationIntelligence.Api.Controllers.Production;

[ApiController]
[Route("api/production/downtimes")]
public class ProductionDowntimesController : BaseApiController
{
    private readonly IProductionDowntimeService _productionDowntimeService;

    public ProductionDowntimesController(IProductionDowntimeService productionDowntimeService)
    {
        _productionDowntimeService = productionDowntimeService;
    }

    [HttpGet("by-execution/{productionExecutionId:guid}")]
    public async Task<IActionResult> GetByProductionExecutionId(Guid productionExecutionId, CancellationToken cancellationToken = default)
    {
        var result = await _productionDowntimeService.GetByProductionExecutionIdAsync(productionExecutionId, cancellationToken);
        return OkResponse(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductionDowntimeRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _productionDowntimeService.CreateAsync(request, User?.Identity?.Name, cancellationToken);
        return CreatedResponse(result);
    }
}
