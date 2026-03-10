using Microsoft.AspNetCore.Mvc;
using OperationIntelligence.Api.Controllers;
using OperationIntelligence.Core;

namespace OperationIntelligence.Api.Controllers.Inventory;

[ApiController]
[Route("api/inventory/stocks")]
public class InventoryStockController : BaseApiController
{
    private readonly IInventoryStockService _inventoryStockService;

    public InventoryStockController(IInventoryStockService inventoryStockService)
    {
        _inventoryStockService = inventoryStockService;
    }

    [HttpPost("stock-in")]
    public async Task<IActionResult> StockIn([FromBody] StockInRequest request, CancellationToken cancellationToken)
    {
        var result = await _inventoryStockService.StockInAsync(request, cancellationToken);
        return OkResponse(result);
    }

    [HttpPost("stock-out")]
    public async Task<IActionResult> StockOut([FromBody] StockOutRequest request, CancellationToken cancellationToken)
    {
        var result = await _inventoryStockService.StockOutAsync(request, cancellationToken);
        return OkResponse(result);
    }

    [HttpGet("product/{productId:guid}")]
    public async Task<IActionResult> GetByProductId(Guid productId, CancellationToken cancellationToken)
    {
        var result = await _inventoryStockService.GetByProductIdAsync(productId, cancellationToken);
        return OkResponse(result);
    }
}
