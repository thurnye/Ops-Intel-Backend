using System.Net;
using System.Text.Json;
using FluentValidation;

namespace OperationIntelligence.Api
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Not Found Error");
                await WriteError(context, HttpStatusCode.NotFound, ex.Message, ErrorCode.NOT_FOUND);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized");
                await WriteError(context, HttpStatusCode.Unauthorized, ex.Message, ErrorCode.AUTHORIZATION_ERROR);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation failed");
                await WriteError(context, HttpStatusCode.BadRequest, ex.Message, ErrorCode.VALIDATION_ERROR);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Business rule violation");
                await WriteError(context, HttpStatusCode.BadRequest, ex.Message, ErrorCode.CONFLICT_ERROR);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled Exception");
                await WriteError(context, HttpStatusCode.InternalServerError, "An unexpected error occurred.", ErrorCode.INTERNAL_ERROR);
            }
        }

        private static async Task WriteError(HttpContext context, HttpStatusCode status, string message, string code)
        {
            context.Response.StatusCode = (int)status;
            context.Response.ContentType = "application/json";

            var response = new ApiResponse<object>
            {
                Data = null,
                Meta = new ApiMeta
                {
                    RequestId = context.TraceIdentifier,
                    Timestamp = DateTime.UtcNow
                },
                Errors = new List<ApiError>
                {
                    new()
                    {
                        Code = code,
                        Message = message
                    }
                }
            };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
