using Microsoft.AspNetCore.Mvc;
using OperationIntelligence.Api.Controllers;
using OperationIntelligence.Core;
using OperationIntelligence.Core.Models.Production.Requests;

namespace OperationIntelligence.Api.Controllers.Production;

[ApiController]
[Route("api/production/material-consumptions")]
public class ProductionMaterialConsumptionsController : BaseApiController
{
    private readonly IProductionMaterialConsumptionService _productionMaterialConsumptionService;

    public ProductionMaterialConsumptionsController(IProductionMaterialConsumptionService productionMaterialConsumptionService)
    {
        _productionMaterialConsumptionService = productionMaterialConsumptionService;
    }

    [HttpGet("by-execution/{productionExecutionId:guid}")]
    public async Task<IActionResult> GetByProductionExecutionId(Guid productionExecutionId, CancellationToken cancellationToken = default)
    {
        var result = await _productionMaterialConsumptionService.GetByProductionExecutionIdAsync(productionExecutionId, cancellationToken);
        return OkResponse(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductionMaterialConsumptionRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _productionMaterialConsumptionService.CreateAsync(request, User?.Identity?.Name, cancellationToken);
        return CreatedResponse(result);
    }
}
