using Microsoft.AspNetCore.Mvc;
using OperationIntelligence.Api.Controllers;
using OperationIntelligence.Core;

namespace OperationIntelligence.Api.Controller.Dashboard;

[Route("api/dashboard")]
public class DashboardController : BaseApiController
{
    private static readonly HashSet<string> AllowedRanges = new(StringComparer.OrdinalIgnoreCase)
    {
        "7d", "30d", "90d", "1y"
    };

    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet("overview")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetOverview(
        [FromQuery] string range = "30d",
        [FromQuery] string site = "all",
        CancellationToken cancellationToken = default)
    {
        if (!AllowedRanges.Contains(range))
        {
            return ErrorResponse(
                StatusCodes.Status400BadRequest,
                ErrorCode.VALIDATION_ERROR,
                "Invalid dashboard range. Allowed values are: 7d, 30d, 90d, 1y.",
                nameof(range));
        }

        var request = new DashboardFilterRequest
        {
            Range = range,
            Site = site
        };

        var result = await _dashboardService.GetOverviewAsync(request, cancellationToken);

        return OkResponse(result);
    }
}