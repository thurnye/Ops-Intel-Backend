using Microsoft.AspNetCore.Mvc;
using OperationIntelligence.Api.Controllers;
using OperationIntelligence.Core;
using OperationIntelligence.Core.Models.Production.Requests;

namespace OperationIntelligence.Api.Controllers.Production;

[ApiController]
[Route("api/production/orders")]
public class ProductionOrdersController : BaseApiController
{
    private readonly IProductionOrderService _productionOrderService;

    public ProductionOrdersController(IProductionOrderService productionOrderService)
    {
        _productionOrderService = productionOrderService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var result = await _productionOrderService.GetPagedAsync(pageNumber, pageSize, cancellationToken);
        return PagedOkResponse(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _productionOrderService.GetByIdAsync(id, cancellationToken);
        if (result is null)
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, "Production order not found.");

        return OkResponse(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductionOrderRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _productionOrderService.CreateAsync(request, User?.Identity?.Name, cancellationToken);
        return CreatedResponse(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductionOrderRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _productionOrderService.UpdateAsync(id, request, User?.Identity?.Name, cancellationToken);
        if (result is null)
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, "Production order not found.");

        return OkResponse(result);
    }

    [HttpPost("{id:guid}/release")]
    public async Task<IActionResult> Release(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _productionOrderService.ReleaseAsync(id, User?.Identity?.Name, cancellationToken);
        if (!result)
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, "Production order not found.");

        return OkResponse(new { message = "Production order released successfully." });
    }

    [HttpPost("{id:guid}/start")]
    public async Task<IActionResult> Start(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _productionOrderService.StartAsync(id, User?.Identity?.Name, cancellationToken);
        if (!result)
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, "Production order not found.");

        return OkResponse(new { message = "Production order started successfully." });
    }

    [HttpPost("{id:guid}/complete")]
    public async Task<IActionResult> Complete(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _productionOrderService.CompleteAsync(id, User?.Identity?.Name, cancellationToken);
        if (!result)
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, "Production order not found.");

        return OkResponse(new { message = "Production order completed successfully." });
    }

    [HttpPost("{id:guid}/close")]
    public async Task<IActionResult> Close(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _productionOrderService.CloseAsync(id, User?.Identity?.Name, cancellationToken);
        if (!result)
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, "Production order not found.");

        return OkResponse(new { message = "Production order closed successfully." });
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _productionOrderService.CancelAsync(id, User?.Identity?.Name, cancellationToken);
        if (!result)
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, "Production order not found.");

        return OkResponse(new { message = "Production order cancelled successfully." });
    }
}
