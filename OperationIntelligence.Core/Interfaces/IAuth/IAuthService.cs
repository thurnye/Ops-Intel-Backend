

namespace OperationIntelligence.Core
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(
            RegisterRequest request,
            CancellationToken cancellationToken = default);

        Task<AuthResponse> LoginAsync(
            LoginRequest request,
            CancellationToken cancellationToken = default);

        Task<AuthResponse> RefreshAsync(
            string refreshToken,
            CancellationToken cancellationToken = default);

        Task ForgotPasswordAsync(
            ForgotPasswordRequest request,
            CancellationToken cancellationToken = default);

        Task ResetPasswordAsync(
            ResetPasswordRequest request,
            CancellationToken cancellationToken = default);

        Task VerifyEmailAsync(
            VerifyEmailRequest request,
            CancellationToken cancellationToken = default);

        Task<UserResponse> GetProfileAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

        Task<PagedResponse<SessionResponse>> GetSessionsAsync(
            Guid userId,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task RevokeSessionAsync(
            Guid userId,
            Guid sessionId,
            CancellationToken cancellationToken = default);
    }
}