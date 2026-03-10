using Microsoft.AspNetCore.Mvc;
using OperationIntelligence.Api.Controllers;
using OperationIntelligence.Core;

namespace OperationIntelligence.Api.Controllers.Orders;

[ApiController]
[Route("api/order-payments")]
public class OrderPaymentsController : BaseApiController
{
    private readonly IOrderPaymentService _orderPaymentService;

    public OrderPaymentsController(IOrderPaymentService orderPaymentService)
    {
        _orderPaymentService = orderPaymentService;
    }

    [HttpGet("by-order/{orderId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyList<OrderPaymentResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByOrderId(Guid orderId, CancellationToken cancellationToken)
    {
        var result = await _orderPaymentService.GetByOrderIdAsync(orderId, cancellationToken);
        return OkResponse(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(OrderPaymentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Record([FromBody] RecordOrderPaymentRequest request, CancellationToken cancellationToken)
    {
        var result = await _orderPaymentService.RecordAsync(request, cancellationToken);
        return CreatedResponse(result);
    }

    [HttpPost("refund")]
    [ProducesResponseType(typeof(OrderPaymentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Refund([FromBody] RefundOrderPaymentRequest request, CancellationToken cancellationToken)
    {
        var result = await _orderPaymentService.RefundAsync(request, cancellationToken);
        return OkResponse(result);
    }
}
