using Microsoft.AspNetCore.Mvc;
using OperationIntelligence.Api.Controllers;
using OperationIntelligence.Core;

namespace OperationIntelligence.Api.Controllers.Orders;

[ApiController]
[Route("api/orders")]
public class OrdersController : BaseApiController
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(OrderDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _orderService.GetByIdAsync(id, cancellationToken);
        if (result == null)
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, OrderErrorMessages.OrderNotFound);

        return OkResponse(result);
    }

    [HttpGet("by-order-number/{orderNumber}")]
    [ProducesResponseType(typeof(OrderDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByOrderNumber(string orderNumber, CancellationToken cancellationToken)
    {
        var result = await _orderService.GetByOrderNumberAsync(orderNumber, cancellationToken);
        if (result == null)
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, OrderErrorMessages.OrderNotFound);

        return OkResponse(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<OrderListItemResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaged([FromQuery] OrderQueryRequest request, CancellationToken cancellationToken)
    {
        var result = await _orderService.GetPagedAsync(request, cancellationToken);
        return PagedOkResponse(result);
    }

    [HttpGet("overview-summary")]
    [ProducesResponseType(typeof(OrderOverviewMetricsSummaryResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOverviewSummary(CancellationToken cancellationToken)
    {
        var result = await _orderService.GetOverviewMetricsSummaryAsync(cancellationToken);
        return OkResponse(result);
    }

    [HttpGet("customer-summary")]
    [ProducesResponseType(typeof(OrderCustomerMetricsSummaryResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCustomerSummary(CancellationToken cancellationToken)
    {
        var result = await _orderService.GetCustomerMetricsSummaryAsync(cancellationToken);
        return OkResponse(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var result = await _orderService.CreateAsync(request, cancellationToken);
        return CreatedResponse(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPost("bulk")]
    [ProducesResponseType(typeof(BulkCreateResponse<OrderResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateBulk([FromBody] BulkCreateRequest<CreateOrderRequest> request, CancellationToken cancellationToken)
    {
        var result = await _orderService.CreateBulkAsync(request, cancellationToken);
        return OkResponse(result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateOrderRequest request, CancellationToken cancellationToken)
    {
        var result = await _orderService.UpdateAsync(id, request, cancellationToken);
        return OkResponse(result);
    }

    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangeStatus(Guid id, [FromBody] ChangeOrderStatusRequest request, CancellationToken cancellationToken)
    {
        var result = await _orderService.ChangeStatusAsync(id, request, cancellationToken);
        return OkResponse(result);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _orderService.DeleteAsync(id, cancellationToken);
        return OkResponse<object?>(null);
    }
}
