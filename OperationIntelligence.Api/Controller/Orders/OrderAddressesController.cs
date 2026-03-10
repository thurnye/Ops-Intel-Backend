using Microsoft.AspNetCore.Mvc;
using OperationIntelligence.Api.Controllers;
using OperationIntelligence.Core;

namespace OperationIntelligence.Api.Controllers.Orders;

[ApiController]
[Route("api/order-addresses")]
public class OrderAddressesController : BaseApiController
{
    private readonly IOrderAddressService _orderAddressService;

    public OrderAddressesController(IOrderAddressService orderAddressService)
    {
        _orderAddressService = orderAddressService;
    }

    [HttpGet("by-order/{orderId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyList<OrderAddressResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByOrderId(Guid orderId, CancellationToken cancellationToken)
    {
        var result = await _orderAddressService.GetByOrderIdAsync(orderId, cancellationToken);
        return OkResponse(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(OrderAddressResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Add([FromBody] CreateOrderAddressRequest request, CancellationToken cancellationToken)
    {
        var result = await _orderAddressService.AddAsync(request, cancellationToken);
        return CreatedResponse(result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(OrderAddressResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateOrderAddressRequest request, CancellationToken cancellationToken)
    {
        var result = await _orderAddressService.UpdateAsync(id, request, cancellationToken);
        return OkResponse(result);
    }
}
