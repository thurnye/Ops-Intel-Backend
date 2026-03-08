using OperationIntelligence.Core.Helpers;
using OperationIntelligence.Core.Interfaces;
using OperationIntelligence.Core.Models;
using OperationIntelligence.DB.Entities;
using OperationIntelligence.DB.Repositories;

namespace OperationIntelligence.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepo;
        private readonly IRefreshTokenRepository _refreshRepo;
        private readonly JwtService _jwt;

        public AuthService(IAuthRepository authRepo, IRefreshTokenRepository refreshRepo, JwtService jwt)
        {
            _authRepo = authRepo;
            _refreshRepo = refreshRepo;
            _jwt = jwt;
        }

        public async Task<TokenPair> RegisterAsync(RegisterRequest model)
        {
            var user = new PlatformUser
            {
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Avatar = model.Avatar,
                Birthdate = model.Birthdate,
                Gender = model.Gender,
                PhoneNumber = model.PhoneNumber,
                Address = model.Address,
                City = model.City,
                State = model.State,
                Country = model.Country,
                PostalCode = model.PostalCode
            };

            var createdUser = await _authRepo.CreateUserAsync(user, model.Password);

            var accessToken = _jwt.GenerateAccessToken(createdUser);
            var refreshToken = _jwt.GenerateRefreshToken();

            await _refreshRepo.CreateAsync(new RefreshToken
            {
                Token = refreshToken,
                UserId = createdUser.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            });

            var userResponse = new User
            {
                Id = createdUser.Id,
                FirstName = createdUser.FirstName,
                LastName = createdUser.LastName,
                Email = createdUser.Email ?? string.Empty,
                Avatar = createdUser.Avatar,
                Birthdate = createdUser.Birthdate,
                Gender = createdUser.Gender,
                PhoneNumber = createdUser.PhoneNumber ?? string.Empty,
                Address = createdUser.Address,
                City = createdUser.City,
                State = createdUser.State,
                Country = createdUser.Country,
                PostalCode = createdUser.PostalCode
            };

            return new TokenPair
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                User = userResponse
            };
        }

        public async Task<TokenPair> LoginAsync(LoginRequest model)
        {
            var user = await _authRepo.FindByEmailAsync(model.Email)
                ?? throw new UnauthorizedAccessException("Invalid credentials.");

            var valid = await _authRepo.CheckPasswordAsync(user, model.Password);
            if (!valid)
                throw new UnauthorizedAccessException("Invalid credentials.");

            var accessToken = _jwt.GenerateAccessToken(user);
            var refreshToken = _jwt.GenerateRefreshToken();

            await _refreshRepo.CreateAsync(new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            });

            var userResponse = new User
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? string.Empty,
                Avatar = user.Avatar,
                Birthdate = user.Birthdate,
                Gender = user.Gender,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                Address = user.Address,
                City = user.City,
                State = user.State,
                Country = user.Country,
                PostalCode = user.PostalCode
            };

            return new TokenPair
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                User = userResponse
            };
        }

        public async Task<TokenPair> RefreshTokensAsync(string refreshToken)
        {
            var existing = await _refreshRepo.GetByTokenAsync(refreshToken);
            if (existing == null || existing.ExpiresAt < DateTime.UtcNow || existing.Revoked)
                throw new UnauthorizedAccessException("Invalid refresh token.");

            if (existing.User == null)
                throw new UnauthorizedAccessException("User not found for refresh token.");

            var newAccess = _jwt.GenerateAccessToken(existing.User);
            var newRefresh = _jwt.GenerateRefreshToken();

            await _refreshRepo.ReplaceAsync(existing, new RefreshToken
            {
                Token = newRefresh,
                UserId = existing.UserId,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            });

            var userResponse = new User
            {
                Id = existing.User.Id,
                FirstName = existing.User.FirstName ?? string.Empty,
                LastName = existing.User.LastName ?? string.Empty,
                Email = existing.User.Email ?? string.Empty,
                Avatar = existing.User.Avatar ?? string.Empty,
                Birthdate = existing.User.Birthdate,
                Gender = existing.User.Gender ?? string.Empty,
                PhoneNumber = existing.User.PhoneNumber ?? string.Empty,
                Address = existing.User.Address ?? string.Empty,
                City = existing.User.City ?? string.Empty,
                State = existing.User.State ?? string.Empty,
                Country = existing.User.Country ?? string.Empty,
                PostalCode = existing.User.PostalCode ?? string.Empty
            };

            return new TokenPair
            {
                AccessToken = newAccess,
                RefreshToken = newRefresh,
                User = userResponse
            };
        }
    }
}
