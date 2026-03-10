using Microsoft.AspNetCore.Mvc;
using OperationIntelligence.Api;
using OperationIntelligence.Core;

namespace OperationIntelligence.Api.Controllers;

[ApiController]
public abstract class BaseApiController : ControllerBase
{
    protected ApiMeta CreateMeta(PaginationMeta? pagination = null)
    {
        return new ApiMeta
        {
            RequestId = HttpContext.TraceIdentifier,
            Timestamp = DateTime.UtcNow,
            Pagination = pagination
        };
    }

    protected IActionResult OkResponse<T>(T data)
    {
        return Ok(new ApiResponse<T>
        {
            Data = data,
            Meta = CreateMeta(),
            Errors = null
        });
    }

    protected IActionResult CreatedResponse<T>(T data)
    {
        return StatusCode(StatusCodes.Status201Created, new ApiResponse<T>
        {
            Data = data,
            Meta = CreateMeta(),
            Errors = null
        });
    }

    protected IActionResult CreatedResponse<T>(string actionName, object routeValues, T data)
    {
        return CreatedAtAction(actionName, routeValues, new ApiResponse<T>
        {
            Data = data,
            Meta = CreateMeta(),
            Errors = null
        });
    }

    protected IActionResult PagedOkResponse<T>(PagedResponse<T> paged)
    {
        return Ok(new ApiResponse<IReadOnlyList<T>>
        {
            Data = paged.Items,
            Meta = CreateMeta(new PaginationMeta
            {
                Page = paged.PageNumber,
                Limit = paged.PageSize,
                Total = paged.TotalRecords,
                TotalPages = paged.TotalPages
            }),
            Errors = null
        });
    }

    protected IActionResult ErrorResponse(int statusCode, string code, string message, string? field = null)
    {
        return StatusCode(statusCode, new ApiResponse<object>
        {
            Data = null,
            Meta = CreateMeta(),
            Errors = new List<ApiError>
            {
                new()
                {
                    Code = code,
                    Message = message,
                    Field = field
                }
            }
        });
    }
}
