using Microsoft.AspNetCore.Mvc;
using OperationIntelligence.Api.Controllers;
using OperationIntelligence.Core;

namespace OperationIntelligence.Api.Controller.Dashboard;

[Route("api/dashboard")]
public class DashboardController : BaseApiController
{
    // private static readonly HashSet<string> AllowedRanges = new(StringComparer.OrdinalIgnoreCase)
    // {
    //     "7d", "30d", "90d", "1y"
    // };

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
        [FromQuery] OverviewFilter filter,
        CancellationToken cancellationToken = default)
    {


        var request = new DashboardFilterRequest
        {
            Range = new DateRange
            {
                From = filter.From,
                To = filter.To
            },
            Site = filter.Site
        };

        var result = await _dashboardService.GetOverviewAsync(request, cancellationToken);

        return OkResponse(result);
    }
}