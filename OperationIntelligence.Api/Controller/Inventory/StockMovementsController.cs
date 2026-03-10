using Microsoft.AspNetCore.Mvc;
using OperationIntelligence.Api.Controllers;
using OperationIntelligence.Core;

namespace OperationIntelligence.Api.Controllers.Inventory;

[ApiController]
[Route("api/inventory/stock-movements")]
public class StockMovementsController : BaseApiController
{
    private readonly IStockMovementService _stockMovementService;

    public StockMovementsController(IStockMovementService stockMovementService)
    {
        _stockMovementService = stockMovementService;
    }

    [HttpGet("product/{productId:guid}")]
    public async Task<IActionResult> GetByProductId(Guid productId, CancellationToken cancellationToken)
    {
        var result = await _stockMovementService.GetByProductIdAsync(productId, cancellationToken);
        return OkResponse(result);
    }

    [HttpGet("warehouse/{warehouseId:guid}")]
    public async Task<IActionResult> GetByWarehouseId(Guid warehouseId, CancellationToken cancellationToken)
    {
        var result = await _stockMovementService.GetByWarehouseIdAsync(warehouseId, cancellationToken);
        return OkResponse(result);
    }
}
