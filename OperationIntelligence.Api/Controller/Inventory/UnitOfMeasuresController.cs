using Microsoft.AspNetCore.Mvc;
using OperationIntelligence.Api.Controllers;
using OperationIntelligence.Api.Models;
using OperationIntelligence.Core;

namespace OperationIntelligence.Api.Controllers.Inventory;

[ApiController]
[Route("api/inventory/unit-of-measures")]
public class UnitOfMeasuresController : BaseApiController
{
    private readonly IUnitOfMeasureService _unitOfMeasureService;

    public UnitOfMeasuresController(IUnitOfMeasureService unitOfMeasureService)
    {
        _unitOfMeasureService = unitOfMeasureService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUnitOfMeasureRequest request, CancellationToken cancellationToken)
    {
        var result = await _unitOfMeasureService.CreateAsync(request, cancellationToken);
        return CreatedResponse(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _unitOfMeasureService.GetByIdAsync(id, cancellationToken);
        return result == null ? ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, "Resource not found.") : OkResponse(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _unitOfMeasureService.GetAllAsync(cancellationToken);
        return OkResponse(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUnitOfMeasureRequest request, CancellationToken cancellationToken)
    {
        if (id != request.Id)
            return ErrorResponse(StatusCodes.Status400BadRequest, ErrorCode.VALIDATION_ERROR, "Route id does not match request id.");

        var result = await _unitOfMeasureService.UpdateAsync(request, cancellationToken);
        return result == null ? ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, "Resource not found.") : OkResponse(result);
    }
}
