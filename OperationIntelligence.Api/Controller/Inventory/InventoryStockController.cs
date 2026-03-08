using Microsoft.AspNetCore.Mvc;
using OperationIntelligence.Core;

namespace OperationIntelligence.Api.Controllers.Inventory;

[ApiController]
[Route("api/inventory/stocks")]
public class InventoryStockController : ControllerBase
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
        return Ok(result);
    }

    [HttpPost("stock-out")]
    public async Task<IActionResult> StockOut([FromBody] StockOutRequest request, CancellationToken cancellationToken)
    {
        var result = await _inventoryStockService.StockOutAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("product/{productId:guid}")]
    public async Task<IActionResult> GetByProductId(Guid productId, CancellationToken cancellationToken)
    {
        var result = await _inventoryStockService.GetByProductIdAsync(productId, cancellationToken);
        return Ok(result);
    }
}
