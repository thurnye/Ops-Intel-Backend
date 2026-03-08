using Microsoft.AspNetCore.Mvc;
using OperationIntelligence.Core;

namespace OperationIntelligence.Api.Controllers.Inventory;

[ApiController]
[Route("api/inventory/product-images")]
public class ProductImagesController : ControllerBase
{
    private readonly IProductImageService _productImageService;

    public ProductImagesController(IProductImageService productImageService)
    {
        _productImageService = productImageService;
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] AddProductImageRequest request, CancellationToken cancellationToken)
    {
        var result = await _productImageService.AddAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("product/{productId:guid}")]
    public async Task<IActionResult> GetByProductId(Guid productId, CancellationToken cancellationToken)
    {
        var result = await _productImageService.GetByProductIdAsync(productId, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{imageId:guid}/set-primary")]
    public async Task<IActionResult> SetPrimary(Guid imageId, CancellationToken cancellationToken)
    {
        var result = await _productImageService.SetPrimaryAsync(imageId, cancellationToken);
        return result ? Ok(new { message = "Primary image updated successfully." }) : NotFound();
    }

    [HttpDelete("{imageId:guid}")]
    public async Task<IActionResult> Delete(Guid imageId, CancellationToken cancellationToken)
    {
        var result = await _productImageService.DeleteAsync(imageId, cancellationToken);
        return result ? NoContent() : NotFound();
    }
}
