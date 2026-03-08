using OperationIntelligence.Api.Models;
using OperationIntelligence.Core.Interfaces;
using OperationIntelligence.Core.Models;
using OperationIntelligence.Core.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OperationIntelligence.Api.Controllers.Auth;

[ApiController]
[Route("api/[controller]")]
public class AuthController : BaseApiController
{
    private readonly IAuthService _authService;
    private readonly BotDetectionService _botDetection;

    public AuthController(IAuthService authService, BotDetectionService botDetection)
    {
        _authService = authService;
        _botDetection = botDetection;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest model)
    {
        if (_botDetection.IsSuspiciousRequest(Request))
        {
            return ErrorResponse(
                StatusCodes.Status400BadRequest,
                ErrorCode.VALIDATION_ERROR,
                "Suspicious or automated request detected. Please try again manually.");
        }

        var response = await _authService.RegisterAsync(model);
        var accessToken = response.AccessToken;
        var refreshToken = response.RefreshToken;

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTime.UtcNow.AddDays(7)
        };

        Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        Response.Headers.Append("X-Access-Token", accessToken);
        Response.Headers.Append("Access-Control-Expose-Headers", "X-Access-Token");

        return OkResponse<object>(response.User);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest model)
    {
        var response = await _authService.LoginAsync(model);
        var accessToken = response.AccessToken;
        var refreshToken = response.RefreshToken;

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTime.UtcNow.AddDays(7)
        };

        Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        Response.Headers.Append("X-Access-Token", accessToken);
        Response.Headers.Append("Access-Control-Expose-Headers", "X-Access-Token");

        return OkResponse<object>(response.User);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
        {
            return ErrorResponse(
                StatusCodes.Status401Unauthorized,
                ErrorCode.AUTHENTICATION_ERROR,
                "Missing refresh token");
        }

        try
        {
            var tokens = await _authService.RefreshTokensAsync(refreshToken);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(7)
            };

            Response.Cookies.Append("refreshToken", tokens.RefreshToken, cookieOptions);
            Response.Headers.Append("X-Access-Token", tokens.AccessToken);
            Response.Headers.Append("Access-Control-Expose-Headers", "X-Access-Token");

            return OkResponse<object>(tokens.User);
        }
        catch
        {
            return ErrorResponse(
                StatusCodes.Status401Unauthorized,
                ErrorCode.AUTHENTICATION_ERROR,
                "Invalid refresh token");
        }
    }

    [Authorize]
    [HttpGet("profile")]
    public IActionResult Profile()
    {
        var email = User.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
        return OkResponse(new { Email = email });
    }
}
