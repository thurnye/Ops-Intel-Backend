using Microsoft.AspNetCore.Mvc;
using OperationIntelligence.Api.Controllers;
using OperationIntelligence.Core;
using OperationIntelligence.Core.Models.Production.Requests;

namespace OperationIntelligence.Api.Controllers.Production;

[ApiController]
[Route("api/production/quality-checks")]
public class ProductionQualityChecksController : BaseApiController
{
    private readonly IProductionQualityCheckService _productionQualityCheckService;

    public ProductionQualityChecksController(IProductionQualityCheckService productionQualityCheckService)
    {
        _productionQualityCheckService = productionQualityCheckService;
    }

    [HttpGet("by-production-order/{productionOrderId:guid}")]
    public async Task<IActionResult> GetByProductionOrderId(Guid productionOrderId, CancellationToken cancellationToken = default)
    {
        var result = await _productionQualityCheckService.GetByProductionOrderIdAsync(productionOrderId, cancellationToken);
        return OkResponse(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductionQualityCheckRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _productionQualityCheckService.CreateAsync(request, User?.Identity?.Name, cancellationToken);
        return CreatedResponse(result);
    }
}
