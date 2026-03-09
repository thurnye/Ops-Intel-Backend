using Microsoft.AspNetCore.Mvc;
using OperationIntelligence.Api.Controllers;
using OperationIntelligence.Api.Models;
using OperationIntelligence.Core;
using OperationIntelligence.Core.Models.Production.Requests;

namespace OperationIntelligence.Api.Controllers.Production;

[ApiController]
[Route("api/production/bills-of-material")]
public class BillsOfMaterialController : BaseApiController
{
    private readonly IBillOfMaterialService _billOfMaterialService;

    public BillsOfMaterialController(IBillOfMaterialService billOfMaterialService)
    {
        _billOfMaterialService = billOfMaterialService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var result = await _billOfMaterialService.GetPagedAsync(pageNumber, pageSize, cancellationToken);
        return PagedOkResponse(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _billOfMaterialService.GetByIdAsync(id, cancellationToken);
        if (result is null)
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, "Bill of material not found.");

        return OkResponse(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBillOfMaterialRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _billOfMaterialService.CreateAsync(request, User?.Identity?.Name, cancellationToken);
        return CreatedResponse(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPost("{billOfMaterialId:guid}/items")]
    public async Task<IActionResult> AddItem(Guid billOfMaterialId, [FromBody] CreateBillOfMaterialItemRequest request, CancellationToken cancellationToken = default)
    {
        request.BillOfMaterialId = billOfMaterialId;
        var result = await _billOfMaterialService.AddItemAsync(request, User?.Identity?.Name, cancellationToken);
        return CreatedResponse(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        var deleted = await _billOfMaterialService.DeleteAsync(id, User?.Identity?.Name, cancellationToken);
        if (!deleted)
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, "Bill of material not found.");

        return OkResponse(new { message = "Bill of material deleted successfully." });
    }
}
