using Microsoft.AspNetCore.Mvc;
using OperationIntelligence.Api.Controllers;
using OperationIntelligence.Core;
using OperationIntelligence.Core.Models.Production.Requests;

namespace OperationIntelligence.Api.Controllers.Production;

[ApiController]
[Route("api/production/outputs")]
public class ProductionOutputsController : BaseApiController
{
    private readonly IProductionOutputService _productionOutputService;

    public ProductionOutputsController(IProductionOutputService productionOutputService)
    {
        _productionOutputService = productionOutputService;
    }

    [HttpGet("by-production-order/{productionOrderId:guid}")]
    public async Task<IActionResult> GetByProductionOrderId(Guid productionOrderId, CancellationToken cancellationToken = default)
    {
        var result = await _productionOutputService.GetByProductionOrderIdAsync(productionOrderId, cancellationToken);
        return OkResponse(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductionOutputRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _productionOutputService.CreateAsync(request, User?.Identity?.Name, cancellationToken);
        return CreatedResponse(result);
    }
}
