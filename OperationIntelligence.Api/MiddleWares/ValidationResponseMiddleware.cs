using OperationIntelligence.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace OperationIntelligence.Api.Middlewares
{
    public class ValidationResponseFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState
                    .Where(ms => ms.Value?.Errors.Count > 0)
                    .Select(ms => new ApiError
                    {
                        Code = ErrorCode.VALIDATION_ERROR,
                        Message = ms.Value?.Errors.First().ErrorMessage ?? "Invalid input.",
                        Field = ms.Key
                    })
                    .ToList();

                var response = new ApiResponse<object>
                {
                    Data = null,
                    Meta = new ApiMeta
                    {
                        RequestId = context.HttpContext.TraceIdentifier,
                        Timestamp = DateTime.UtcNow
                    },
                    Errors = errors
                };

                context.Result = new BadRequestObjectResult(response);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // nothing needed here
        }
    }
}
