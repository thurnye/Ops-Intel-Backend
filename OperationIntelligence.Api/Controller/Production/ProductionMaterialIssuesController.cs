using Microsoft.AspNetCore.Mvc;
using OperationIntelligence.Api.Controllers;
using OperationIntelligence.Core;
using OperationIntelligence.Core.Models.Production.Requests;

namespace OperationIntelligence.Api.Controllers.Production;

[ApiController]
[Route("api/production/material-issues")]
public class ProductionMaterialIssuesController : BaseApiController
{
    private readonly IProductionMaterialIssueService _productionMaterialIssueService;

    public ProductionMaterialIssuesController(IProductionMaterialIssueService productionMaterialIssueService)
    {
        _productionMaterialIssueService = productionMaterialIssueService;
    }

    [HttpGet("by-production-order/{productionOrderId:guid}")]
    public async Task<IActionResult> GetByProductionOrderId(Guid productionOrderId, CancellationToken cancellationToken = default)
    {
        var result = await _productionMaterialIssueService.GetByProductionOrderIdAsync(productionOrderId, cancellationToken);
        return OkResponse(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductionMaterialIssueRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _productionMaterialIssueService.CreateAsync(request, User?.Identity?.Name, cancellationToken);
        return CreatedResponse(result);
    }
}
