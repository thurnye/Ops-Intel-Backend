using Microsoft.AspNetCore.Mvc;
using OperationIntelligence.Api.Controllers;
using OperationIntelligence.Api.Models;
using OperationIntelligence.Core;

namespace OperationIntelligence.Api.Controllers.Inventory;

[ApiController]
[Route("api/inventory/categories")]
public class CategoriesController : BaseApiController
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        var result = await _categoryService.CreateAsync(request, cancellationToken);
        return CreatedResponse(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _categoryService.GetByIdAsync(id, cancellationToken);
        return result == null ? ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, "Resource not found.") : OkResponse(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _categoryService.GetAllAsync(cancellationToken);
        return OkResponse(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        if (id != request.Id)
            return ErrorResponse(StatusCodes.Status400BadRequest, ErrorCode.VALIDATION_ERROR, "Route id does not match request id.");

        var result = await _categoryService.UpdateAsync(request, cancellationToken);
        return result == null ? ErrorResponse(StatusCodes.Status404NotFound, ErrorCode.NOT_FOUND, "Resource not found.") : OkResponse(result);
    }
}
