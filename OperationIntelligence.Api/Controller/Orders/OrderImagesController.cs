using Microsoft.AspNetCore.Mvc;
using OperationIntelligence.Api.Controllers;
using OperationIntelligence.Core;

namespace OperationIntelligence.Api.Controllers.Orders;

[ApiController]
[Route("api/order-images")]
public class OrderImagesController : BaseApiController
{
    private readonly IOrderImageService _orderImageService;

    public OrderImagesController(IOrderImageService orderImageService)
    {
        _orderImageService = orderImageService;
    }

    [HttpGet("by-order/{orderId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyList<OrderImageResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByOrderId(Guid orderId, CancellationToken cancellationToken)
    {
        var result = await _orderImageService.GetByOrderIdAsync(orderId, cancellationToken);
        return OkResponse(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(OrderImageResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Add([FromBody] CreateOrderImageRequest request, CancellationToken cancellationToken)
    {
        var result = await _orderImageService.AddAsync(request, cancellationToken);
        return CreatedResponse(result);
    }

    [HttpPatch("{imageId:guid}/set-primary")]
    [ProducesResponseType(typeof(OrderImageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SetPrimary(Guid imageId, CancellationToken cancellationToken)
    {
        var result = await _orderImageService.SetPrimaryAsync(imageId, cancellationToken);
        return OkResponse(result);
    }

    [HttpDelete("{imageId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remove(Guid imageId, CancellationToken cancellationToken)
    {
        await _orderImageService.RemoveAsync(imageId, cancellationToken);
        return OkResponse<object?>(null);
    }
}
