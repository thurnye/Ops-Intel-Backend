using Microsoft.AspNetCore.Mvc;
using OperationIntelligence.Api.Controllers;
using OperationIntelligence.Core;

namespace OperationIntelligence.Api.Controllers.Inventory;

[ApiController]
[Route("api/inventory/product-suppliers")]
public class ProductSuppliersController : BaseApiController
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
        return OkResponse(result);
    }

    [HttpGet("product/{productId:guid}")]
    public async Task<IActionResult> GetByProductId(Guid productId, CancellationToken cancellationToken)
    {
        var result = await _productSupplierService.GetByProductIdAsync(productId, cancellationToken);
        return OkResponse(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductSupplierRequest request, CancellationToken cancellationToken)
    {
        if (id != request.Id)
            return ErrorResponse(StatusCodes.Status400BadRequest, ErrorCode.VALIDATION_ERROR, "Route id does not match request id.");

        var result = await _productSupplierService.UpdateAsync(request, cancellationToken);
        return result == null ? ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, "Resource not found.") : OkResponse(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Remove(Guid id, CancellationToken cancellationToken)
    {
        var result = await _productSupplierService.RemoveAsync(id, cancellationToken);
        return result
            ? OkResponse<object?>(null)
            : ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, "Resource not found.");
    }
}
