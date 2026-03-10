using Microsoft.AspNetCore.Mvc;
using OperationIntelligence.Api.Controllers;
using OperationIntelligence.Core;

namespace OperationIntelligence.Api.Controllers.Production;

[ApiController]
[Route("api/production/bill-of-material-items")]
public class BillOfMaterialItemsController : BaseApiController
{
    private readonly IBillOfMaterialItemService _billOfMaterialItemService;

    public BillOfMaterialItemsController(IBillOfMaterialItemService billOfMaterialItemService)
    {
        _billOfMaterialItemService = billOfMaterialItemService;
    }

    [HttpGet("by-bill-of-material/{billOfMaterialId:guid}")]
    public async Task<IActionResult> GetByBillOfMaterialId(Guid billOfMaterialId, CancellationToken cancellationToken = default)
    {
        var result = await _billOfMaterialItemService.GetByBillOfMaterialIdAsync(billOfMaterialId, cancellationToken);
        return OkResponse(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _billOfMaterialItemService.GetByIdAsync(id, cancellationToken);
        if (result is null)
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, "Bill of material item not found.");

        return OkResponse(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        var deleted = await _billOfMaterialItemService.DeleteAsync(id, User?.Identity?.Name, cancellationToken);
        if (!deleted)
            return ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, "Bill of material item not found.");

        return OkResponse(new { message = "Bill of material item deleted successfully." });
    }
}
