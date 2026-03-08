using Microsoft.AspNetCore.Mvc;
using OperationIntelligence.Core;

namespace OperationIntelligence.Api.Controllers.Inventory;

[ApiController]
[Route("api/inventory/product-suppliers")]
public class ProductSuppliersController : ControllerBase
{
    private readonly IProductSupplierService _productSupplierService;

    public ProductSuppliersController(IProductSupplierService productSupplierService)
    {
        _productSupplierService = productSupplierService;
    }

    [HttpPost]
    public async Task<IActionResult> Assign([FromBody] AssignProductSupplierRequest request, CancellationToken cancellationToken)
    {
        var result = await _productSupplierService.AssignAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("product/{productId:guid}")]
    public async Task<IActionResult> GetByProductId(Guid productId, CancellationToken cancellationToken)
    {
        var result = await _productSupplierService.GetByProductIdAsync(productId, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductSupplierRequest request, CancellationToken cancellationToken)
    {
        if (id != request.Id)
            return BadRequest("Route id does not match request id.");

        var result = await _productSupplierService.UpdateAsync(request, cancellationToken);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Remove(Guid id, CancellationToken cancellationToken)
    {
        var result = await _productSupplierService.RemoveAsync(id, cancellationToken);
        return result ? NoContent() : NotFound();
    }
}
