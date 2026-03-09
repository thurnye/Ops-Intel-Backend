using Microsoft.AspNetCore.Mvc;
using OperationIntelligence.Api.Controllers;
using OperationIntelligence.Core;
using OperationIntelligence.Core.Models.Production.Requests;

namespace OperationIntelligence.Api.Controllers.Production;

[ApiController]
[Route("api/production/scraps")]
public class ProductionScrapsController : BaseApiController
{
    private readonly IProductionScrapService _productionScrapService;

    public ProductionScrapsController(IProductionScrapService productionScrapService)
    {
        _productionScrapService = productionScrapService;
    }

    [HttpGet("by-production-order/{productionOrderId:guid}")]
    public async Task<IActionResult> GetByProductionOrderId(Guid productionOrderId, CancellationToken cancellationToken = default)
    {
        var result = await _productionScrapService.GetByProductionOrderIdAsync(productionOrderId, cancellationToken);
        return OkResponse(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductionScrapRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _productionScrapService.CreateAsync(request, User?.Identity?.Name, cancellationToken);
        return CreatedResponse(result);
    }
}
