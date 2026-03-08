using Microsoft.AspNetCore.Mvc;
using OperationIntelligence.Api.Controllers;
using OperationIntelligence.Api.Models;
using OperationIntelligence.Core;

namespace OperationIntelligence.Api.Controllers.Orders;

[ApiController]
[Route("api/order-items")]
public class OrderItemsController : BaseApiController
{
    private readonly IOrderItemService _orderItemService;

    public OrderItemsController(IOrderItemService orderItemService)
    {
        _orderItemService = orderItemService;
    }

    [HttpGet("by-order/{orderId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyList<OrderItemResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByOrderId(Guid orderId, CancellationToken cancellationToken)
    {
        var result = await _orderItemService.GetByOrderIdAsync(orderId, cancellationToken);
        return OkResponse(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(OrderItemResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Add([FromBody] CreateOrderItemRequest request, CancellationToken cancellationToken)
    {
        var result = await _orderItemService.AddAsync(request, cancellationToken);
        return CreatedResponse(result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(OrderItemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateOrderItemRequest request, CancellationToken cancellationToken)
    {
        var result = await _orderItemService.UpdateAsync(id, request, cancellationToken);
        return OkResponse(result);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remove(Guid id, CancellationToken cancellationToken)
    {
        await _orderItemService.RemoveAsync(id, cancellationToken);
        return OkResponse<object?>(null);
    }
}
