using Microsoft.AspNetCore.Mvc;
using OperationIntelligence.Api.Controllers;
using OperationIntelligence.Core;

namespace OperationIntelligence.Api.Controllers.Orders;

[ApiController]
[Route("api/order-notes")]
public class OrderNotesController : BaseApiController
{
    private readonly IOrderNoteService _orderNoteService;

    public OrderNotesController(IOrderNoteService orderNoteService)
    {
        _orderNoteService = orderNoteService;
    }

    [HttpGet("by-order/{orderId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyList<OrderNoteResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByOrderId(Guid orderId, CancellationToken cancellationToken)
    {
        var result = await _orderNoteService.GetByOrderIdAsync(orderId, cancellationToken);
        return OkResponse(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(OrderNoteResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Add([FromBody] CreateOrderNoteRequest request, CancellationToken cancellationToken)
    {
        var result = await _orderNoteService.AddAsync(request, cancellationToken);
        return CreatedResponse(result);
    }
}
