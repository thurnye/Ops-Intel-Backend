using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OperationIntelligence.Core;

namespace OperationIntelligence.Api.Controllers.Auth
{
    [Route("api/[controller]")]
    public class AuthController : BaseApiController
    {
        private const string RefreshTokenCookieName = "refreshToken";

        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Register(
            [FromBody] RegisterRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _authService.RegisterAsync(request, cancellationToken);

            AppendRefreshTokenCookie(response.RefreshToken);

            return OkResponse(new
            {
                AccessToken = response.AccessToken,
                AccessTokenExpiresAtUtc = response.AccessTokenExpiresAtUtc,
                User = response.User
            });
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login(
            [FromBody] LoginRequest request,
            CancellationToken cancellationToken)
        {
            var response = await _authService.LoginAsync(request, cancellationToken);

            AppendRefreshTokenCookie(response.RefreshToken);

            return OkResponse(new
            {
                AccessToken = response.AccessToken,
                AccessTokenExpiresAtUtc = response.AccessTokenExpiresAtUtc,
                User = response.User
            });
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Refresh(CancellationToken cancellationToken)
        {
            if (!Request.Cookies.TryGetValue(RefreshTokenCookieName, out var refreshToken) ||
                string.IsNullOrWhiteSpace(refreshToken))
            {
                return ErrorResponse(
                    StatusCodes.Status401Unauthorized,
                    ErrorCode.AUTHENTICATION_ERROR,
                    "Missing refresh token.");
            }

            var response = await _authService.RefreshAsync(refreshToken, cancellationToken);

            AppendRefreshTokenCookie(response.RefreshToken);

            return OkResponse(new
            {
                AccessToken = response.AccessToken,
                AccessTokenExpiresAtUtc = response.AccessTokenExpiresAtUtc,
                User = response.User
            });
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ForgotPassword(
            [FromBody] ForgotPasswordRequest request,
            CancellationToken cancellationToken)
        {
            await _authService.ForgotPasswordAsync(request, cancellationToken);

            return OkResponse(new
            {
                Message = "If the account exists, password reset instructions have been initiated."
            });
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ResetPassword(
            [FromBody] ResetPasswordRequest request,
            CancellationToken cancellationToken)
        {
            await _authService.ResetPasswordAsync(request, cancellationToken);

            DeleteRefreshTokenCookie();

            return OkResponse(new
            {
                Message = "Password has been reset successfully."
            });
        }

        [HttpPost("verify-email")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> VerifyEmail(
            [FromBody] VerifyEmailRequest request,
            CancellationToken cancellationToken)
        {
            await _authService.VerifyEmailAsync(request, cancellationToken);

            return OkResponse(new
            {
                Message = "Email verified successfully."
            });
        }

        [Authorize]
        [HttpGet("profile")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Profile(CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            var user = await _authService.GetProfileAsync(userId, cancellationToken);

            return OkResponse(user);
        }

        [Authorize]
        [HttpGet("sessions")]
        [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<object>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Sessions(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();

            var sessions = await _authService.GetSessionsAsync(
                userId,
                pageNumber,
                pageSize,
                cancellationToken);

            return PagedOkResponse(sessions);
        }

        [Authorize]
        [HttpDelete("sessions/{sessionId:guid}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RevokeSession(
            [FromRoute] Guid sessionId,
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();

            await _authService.RevokeSessionAsync(userId, sessionId, cancellationToken);

            return OkResponse(new
            {
                Message = "Session revoked successfully."
            });
        }

        [Authorize]
        [HttpPost("logout")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        public IActionResult Logout()
        {
            DeleteRefreshTokenCookie();

            return OkResponse(new
            {
                Message = "Logged out successfully."
            });
        }

        private Guid GetCurrentUserId()
        {
            var userIdValue =
                User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                User.FindFirstValue("sub");

            if (string.IsNullOrWhiteSpace(userIdValue) || !Guid.TryParse(userIdValue, out var userId))
            {
                throw new UnauthorizedAccessException("Invalid authenticated user context.");
            }

            return userId;
        }

        private void AppendRefreshTokenCookie(string refreshToken)
        {
            var isHttps = Request.IsHttps;

            var options = new CookieOptions
            {
                HttpOnly = true,
                Secure = isHttps,
                SameSite = isHttps ? SameSiteMode.None : SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddDays(7),
                Path = "/"
            };

            Response.Cookies.Append(RefreshTokenCookieName, refreshToken, options);
        }

        private void DeleteRefreshTokenCookie()
        {
            var isHttps = Request.IsHttps;

            var options = new CookieOptions
            {
                HttpOnly = true,
                Secure = isHttps,
                SameSite = isHttps ? SameSiteMode.None : SameSiteMode.Lax,
                Path = "/"
            };

            Response.Cookies.Delete(RefreshTokenCookieName, options);
        }
    }
}