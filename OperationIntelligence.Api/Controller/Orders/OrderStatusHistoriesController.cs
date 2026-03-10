using Microsoft.AspNetCore.Mvc;
using OperationIntelligence.Api.Controllers;
using OperationIntelligence.Core;

namespace OperationIntelligence.Api.Controllers.Orders;

[ApiController]
[Route("api/order-status-histories")]
public class OrderStatusHistoriesController : BaseApiController
{
    private readonly IOrderStatusHistoryService _orderStatusHistoryService;

    public OrderStatusHistoriesController(IOrderStatusHistoryService orderStatusHistoryService)
    {
        _orderStatusHistoryService = orderStatusHistoryService;
    }

    [HttpGet("by-order/{orderId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyList<OrderStatusHistoryResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByOrderId(Guid orderId, CancellationToken cancellationToken)
    {
        var result = await _orderStatusHistoryService.GetByOrderIdAsync(orderId, cancellationToken);
        return OkResponse(result);
    }

    [HttpGet("by-order/{orderId:guid}/latest")]
    [ProducesResponseType(typeof(OrderStatusHistoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetLatestByOrderId(Guid orderId, CancellationToken cancellationToken)
    {
        var result = await _orderStatusHistoryService.GetLatestByOrderIdAsync(orderId, cancellationToken);
        if (result == null)
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, "No status history found for this order.");

        return OkResponse(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(OrderStatusHistoryResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Add([FromBody] CreateOrderStatusHistoryRequest request, CancellationToken cancellationToken)
    {
        var result = await _orderStatusHistoryService.AddAsync(request, cancellationToken);
        return CreatedResponse(result);
    }
}
