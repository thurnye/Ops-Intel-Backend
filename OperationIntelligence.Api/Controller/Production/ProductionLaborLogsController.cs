using Microsoft.AspNetCore.Mvc;
using OperationIntelligence.Api.Controllers;
using OperationIntelligence.Core;
using OperationIntelligence.Core.Models.Production.Requests;

namespace OperationIntelligence.Api.Controllers.Production;

[ApiController]
[Route("api/production/labor-logs")]
public class ProductionLaborLogsController : BaseApiController
{
    private readonly IProductionLaborLogService _productionLaborLogService;

    public ProductionLaborLogsController(IProductionLaborLogService productionLaborLogService)
    {
        _productionLaborLogService = productionLaborLogService;
    }

    [HttpGet("by-execution/{productionExecutionId:guid}")]
    public async Task<IActionResult> GetByProductionExecutionId(Guid productionExecutionId, CancellationToken cancellationToken = default)
    {
        var result = await _productionLaborLogService.GetByProductionExecutionIdAsync(productionExecutionId, cancellationToken);
        return OkResponse(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductionLaborLogRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _productionLaborLogService.CreateAsync(request, User?.Identity?.Name, cancellationToken);
        return CreatedResponse(result);
    }
}
