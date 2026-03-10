using OperationIntelligence.DB;

namespace OperationIntelligence.Core
{
    public class AuthService : IAuthService
    {
        private readonly IPlatformUserRepository _platformUserRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUserSessionRepository _userSessionRepository;
        private readonly IPasswordResetTokenRepository _passwordResetTokenRepository;
        private readonly IEmailVerificationTokenRepository _emailVerificationTokenRepository;
        private readonly IPasswordHistoryRepository _passwordHistoryRepository;
        private readonly ILoginAttemptRepository _loginAttemptRepository;
        private readonly IMediaFileRepository _mediaFileRepository;
        private readonly INormalizationService _normalizationService;
        private readonly IPasswordService _passwordService;
        private readonly ITokenService _tokenService;

        public AuthService(
            IPlatformUserRepository platformUserRepository,
            IRoleRepository roleRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IUserSessionRepository userSessionRepository,
            IPasswordResetTokenRepository passwordResetTokenRepository,
            IEmailVerificationTokenRepository emailVerificationTokenRepository,
            IPasswordHistoryRepository passwordHistoryRepository,
            ILoginAttemptRepository loginAttemptRepository,
            IMediaFileRepository mediaFileRepository,
            INormalizationService normalizationService,
            IPasswordService passwordService,
            ITokenService tokenService)
        {
            _platformUserRepository = platformUserRepository;
            _roleRepository = roleRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _userSessionRepository = userSessionRepository;
            _passwordResetTokenRepository = passwordResetTokenRepository;
            _emailVerificationTokenRepository = emailVerificationTokenRepository;
            _passwordHistoryRepository = passwordHistoryRepository;
            _loginAttemptRepository = loginAttemptRepository;
            _mediaFileRepository = mediaFileRepository;
            _normalizationService = normalizationService;
            _passwordService = passwordService;
            _tokenService = tokenService;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
        {
            var normalizedEmail = _normalizationService.NormalizeEmail(request.Email);
            var normalizedUserName = string.IsNullOrWhiteSpace(request.UserName)
                ? null
                : _normalizationService.NormalizeUserName(request.UserName);

            if (await _platformUserRepository.EmailExistsAsync(normalizedEmail, cancellationToken))
                throw new InvalidOperationException("A user with this email already exists.");

            if (!string.IsNullOrWhiteSpace(normalizedUserName) &&
                await _platformUserRepository.UserNameExistsAsync(normalizedUserName, cancellationToken))
            {
                throw new InvalidOperationException("A user with this username already exists.");
            }

            if (!_passwordService.IsStrongPassword(request.Password))
                throw new InvalidOperationException("Password does not meet complexity requirements.");

            Guid? avatarFileId = null;
            string avatar = string.Empty;
            if (request.AvatarFileId.HasValue)
            {
                var mediaFiles = await _mediaFileRepository.GetByIdsAsync([request.AvatarFileId.Value], cancellationToken);
                if (mediaFiles.Count > 0)
                {
                    avatarFileId = request.AvatarFileId.Value;
                    var mediaFile = mediaFiles[0];
                    avatar = mediaFile.PublicUrl ?? mediaFile.StoragePath;
                }
            }

            var user = new PlatformUser
            {
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                Email = request.Email.Trim(),
                Avatar = avatar,
                NormalizedEmail = normalizedEmail,
                UserName = request.UserName?.Trim(),
                NormalizedUserName = normalizedUserName,
                PasswordHash = _passwordService.HashPassword(request.Password),
                EmailConfirmed = false,
                IsActive = true,
                IsLocked = false,
                PasswordChangedAtUtc = DateTime.UtcNow,
                AuthProvider = "Local"
            };

            var profile = new PlatformUserProfile
            {
                UserId = user.Id,
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                DisplayName = $"{request.FirstName.Trim()} {request.LastName.Trim()}",
                Birthdate = request.Birthdate,
                Gender = request.Gender,
                PhoneNumber = request.PhoneNumber,
                AddressLine1 = request.AddressLine1,
                AddressLine2 = request.AddressLine2,
                City = request.City,
                StateOrProvince = request.StateOrProvince,
                Country = request.Country,
                PostalCode = request.PostalCode,
                AvatarFileId = avatarFileId
            };

            await _platformUserRepository.AddUserWithProfileAsync(user, profile, cancellationToken);

            var defaultRole = await _roleRepository.GetByNameAsync(RoleNames.Viewer, cancellationToken);
            if (defaultRole != null)
            {
                user.UserRoles.Add(new UserRole
                {
                    UserId = user.Id,
                    RoleId = defaultRole.Id
                });
            }

            await _platformUserRepository.SaveChangesAsync(cancellationToken);

            var rawVerificationToken = _tokenService.GenerateSecureEmailVerificationToken();
            var verificationTokenHash = _tokenService.HashToken(rawVerificationToken);

            await _emailVerificationTokenRepository.AddAsync(new EmailVerificationToken
            {
                UserId = user.Id,
                TokenHash = verificationTokenHash,
                ExpiresAtUtc = DateTime.UtcNow.AddHours(AuthConstants.EmailVerificationLifetimeHours)
            }, cancellationToken);

            var rawRefreshToken = _tokenService.GenerateSecureRefreshToken();
            var refreshTokenHash = _tokenService.HashToken(rawRefreshToken);

            await _refreshTokenRepository.AddAsync(new RefreshToken
            {
                UserId = user.Id,
                TokenHash = refreshTokenHash,
                ExpiresAtUtc = DateTime.UtcNow.AddDays(AuthConstants.RefreshTokenLifetimeDays)
            }, cancellationToken);

            await _passwordHistoryRepository.AddAsync(new PasswordHistory
            {
                UserId = user.Id,
                PasswordHash = user.PasswordHash
            }, cancellationToken);

            await _refreshTokenRepository.SaveChangesAsync(cancellationToken);

            var fullUser = await _platformUserRepository.GetFullAuthUserAsync(user.Id, cancellationToken)
                ?? throw new InvalidOperationException("Unable to load registered user.");

            var userResponse = fullUser.ToUserResponse();
            var accessToken = _tokenService.GenerateAccessToken(fullUser, userResponse.Roles, userResponse.Permissions);

            return new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = rawRefreshToken,
                AccessTokenExpiresAtUtc = _tokenService.GetAccessTokenExpiryUtc(),
                User = userResponse
            };
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
        {
            var normalizedValue = request.EmailOrUserName.Contains("@")
                ? _normalizationService.NormalizeEmail(request.EmailOrUserName)
                : _normalizationService.NormalizeUserName(request.EmailOrUserName);

            var user = await _platformUserRepository.GetByEmailOrUserNameAsync(normalizedValue, cancellationToken);

            if (user == null)
            {
                await _loginAttemptRepository.AddAsync(new LoginAttempt
                {
                    Email = request.EmailOrUserName,
                    IsSuccessful = false,
                    FailureReason = "User not found"
                }, cancellationToken);

                await _loginAttemptRepository.SaveChangesAsync(cancellationToken);
                throw new UnauthorizedAccessException("Invalid credentials.");
            }

            if (!user.IsActive || user.IsDeleted)
                throw new UnauthorizedAccessException("Account is not available.");

            if (user.LockoutEndUtc.HasValue && user.LockoutEndUtc > DateTime.UtcNow)
                throw new UnauthorizedAccessException("Account is temporarily locked.");

            if (user.IsLocked && user.LockoutEndUtc.HasValue && user.LockoutEndUtc <= DateTime.UtcNow)
            {
                user.IsLocked = false;
                user.LockoutEndUtc = null;
                user.AccessFailedCount = 0;
            }

            var passwordValid = _passwordService.VerifyPassword(request.Password, user.PasswordHash);
            if (!passwordValid)
            {
                user.AccessFailedCount += 1;

                await _loginAttemptRepository.AddAsync(new LoginAttempt
                {
                    UserId = user.Id,
                    Email = user.Email,
                    IsSuccessful = false,
                    FailureReason = "Invalid password"
                }, cancellationToken);

                if (user.AccessFailedCount >= AuthConstants.MaxFailedAccessAttempts)
                {
                    user.IsLocked = true;
                    user.LockoutEndUtc = DateTime.UtcNow.AddMinutes(AuthConstants.LockoutMinutes);
                }

                _platformUserRepository.Update(user);
                await _platformUserRepository.SaveChangesAsync(cancellationToken);

                throw new UnauthorizedAccessException("Invalid credentials.");
            }

            user.AccessFailedCount = 0;
            user.IsLocked = false;
            user.LockoutEndUtc = null;
            user.LastLoginAtUtc = DateTime.UtcNow;

            await _loginAttemptRepository.AddAsync(new LoginAttempt
            {
                UserId = user.Id,
                Email = user.Email,
                IsSuccessful = true
            }, cancellationToken);

            var rawRefreshToken = _tokenService.GenerateSecureRefreshToken();
            var refreshTokenHash = _tokenService.HashToken(rawRefreshToken);

            await _refreshTokenRepository.AddAsync(new RefreshToken
            {
                UserId = user.Id,
                TokenHash = refreshTokenHash,
                ExpiresAtUtc = DateTime.UtcNow.AddDays(AuthConstants.RefreshTokenLifetimeDays)
            }, cancellationToken);

            _platformUserRepository.Update(user);
            await _platformUserRepository.SaveChangesAsync(cancellationToken);

            var fullUser = await _platformUserRepository.GetFullAuthUserAsync(user.Id, cancellationToken)
                ?? throw new InvalidOperationException("Unable to load authenticated user.");

            var userResponse = fullUser.ToUserResponse();
            var accessToken = _tokenService.GenerateAccessToken(fullUser, userResponse.Roles, userResponse.Permissions);

            return new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = rawRefreshToken,
                AccessTokenExpiresAtUtc = _tokenService.GetAccessTokenExpiryUtc(),
                User = userResponse
            };
        }

        public async Task<AuthResponse> RefreshAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            var refreshTokenHash = _tokenService.HashToken(refreshToken);

            var existingToken = await _refreshTokenRepository.GetActiveByTokenHashAsync(refreshTokenHash, cancellationToken);
            if (existingToken == null || existingToken.User == null)
                throw new UnauthorizedAccessException("Invalid refresh token.");

            if (!existingToken.User.IsActive || existingToken.User.IsDeleted)
                throw new UnauthorizedAccessException("User account is not available.");

            if (existingToken.User.LockoutEndUtc.HasValue && existingToken.User.LockoutEndUtc > DateTime.UtcNow)
                throw new UnauthorizedAccessException("Account is temporarily locked.");

            var newRefreshToken = new RefreshToken
            {
                UserId = existingToken.UserId,
                TokenHash = _tokenService.HashToken(_tokenService.GenerateSecureRefreshToken()),
                ExpiresAtUtc = DateTime.UtcNow.AddDays(AuthConstants.RefreshTokenLifetimeDays)
            };

            var rawNewRefreshToken = _tokenService.GenerateSecureRefreshToken();
            newRefreshToken.TokenHash = _tokenService.HashToken(rawNewRefreshToken);

            existingToken.RevokedAtUtc = DateTime.UtcNow;

            await _refreshTokenRepository.AddAsync(newRefreshToken, cancellationToken);
            await _refreshTokenRepository.SaveChangesAsync(cancellationToken);

            existingToken.ReplacedByTokenId = newRefreshToken.Id;
            _refreshTokenRepository.Update(existingToken);
            await _refreshTokenRepository.SaveChangesAsync(cancellationToken);

            var fullUser = await _platformUserRepository.GetFullAuthUserAsync(existingToken.UserId, cancellationToken)
                ?? throw new UnauthorizedAccessException("User not found.");

            var userResponse = fullUser.ToUserResponse();
            var accessToken = _tokenService.GenerateAccessToken(fullUser, userResponse.Roles, userResponse.Permissions);

            return new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = rawNewRefreshToken,
                AccessTokenExpiresAtUtc = _tokenService.GetAccessTokenExpiryUtc(),
                User = userResponse
            };
        }

        public async Task ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken cancellationToken = default)
        {
            var normalizedEmail = _normalizationService.NormalizeEmail(request.Email);
            var user = await _platformUserRepository.GetByEmailAsync(normalizedEmail, cancellationToken);

            if (user == null)
                return;

            await _passwordResetTokenRepository.InvalidateAllByUserIdAsync(user.Id, cancellationToken);

            var rawToken = _tokenService.GenerateSecurePasswordResetToken();
            var tokenHash = _tokenService.HashToken(rawToken);

            await _passwordResetTokenRepository.AddAsync(new PasswordResetToken
            {
                UserId = user.Id,
                TokenHash = tokenHash,
                ExpiresAtUtc = DateTime.UtcNow.AddMinutes(AuthConstants.PasswordResetLifetimeMinutes)
            }, cancellationToken);

            await _passwordResetTokenRepository.SaveChangesAsync(cancellationToken);
        }

        public async Task ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default)
        {
            var tokenHash = _tokenService.HashToken(request.Token);
            var resetToken = await _passwordResetTokenRepository.GetActiveByTokenHashAsync(tokenHash, cancellationToken);

            if (resetToken == null || resetToken.User == null)
                throw new UnauthorizedAccessException("Invalid or expired reset token.");

            var recentPasswords = await _passwordHistoryRepository.GetRecentByUserIdAsync(
                resetToken.UserId,
                AuthConstants.PasswordHistoryDepth,
                cancellationToken);

            if (recentPasswords.Any(p => _passwordService.VerifyPassword(request.NewPassword, p.PasswordHash)))
                throw new InvalidOperationException("You cannot reuse a recent password.");

            var newPasswordHash = _passwordService.HashPassword(request.NewPassword);

            resetToken.User.PasswordHash = newPasswordHash;
            resetToken.User.PasswordChangedAtUtc = DateTime.UtcNow;
            resetToken.User.AccessFailedCount = 0;
            resetToken.User.IsLocked = false;
            resetToken.User.LockoutEndUtc = null;

            resetToken.UsedAtUtc = DateTime.UtcNow;

            await _passwordHistoryRepository.AddAsync(new PasswordHistory
            {
                UserId = resetToken.UserId,
                PasswordHash = newPasswordHash
            }, cancellationToken);

            await _refreshTokenRepository.RevokeAllByUserIdAsync(resetToken.UserId, null, cancellationToken);
            await _passwordResetTokenRepository.SaveChangesAsync(cancellationToken);
        }

        public async Task VerifyEmailAsync(VerifyEmailRequest request, CancellationToken cancellationToken = default)
        {
            var tokenHash = _tokenService.HashToken(request.Token);
            var verificationToken = await _emailVerificationTokenRepository.GetActiveByTokenHashAsync(tokenHash, cancellationToken);

            if (verificationToken == null || verificationToken.User == null)
                throw new UnauthorizedAccessException("Invalid or expired verification token.");

            verificationToken.User.EmailConfirmed = true;
            verificationToken.VerifiedAtUtc = DateTime.UtcNow;

            await _emailVerificationTokenRepository.InvalidateAllByUserIdAsync(verificationToken.UserId, cancellationToken);
            await _emailVerificationTokenRepository.SaveChangesAsync(cancellationToken);
        }

        public async Task<UserResponse> GetProfileAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var user = await _platformUserRepository.GetFullAuthUserAsync(userId, cancellationToken)
                ?? throw new KeyNotFoundException("User not found.");

            return user.ToUserResponse();
        }

        public async Task<global::OperationIntelligence.Core.PagedResponse<SessionResponse>> GetSessionsAsync(
            Guid userId,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            var sessions = await _userSessionRepository.GetActiveByUserIdAsync(userId, cancellationToken);

            var totalRecords = sessions.Count;
            var items = sessions
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => x.ToSessionResponse())
                .ToList();

            return new global::OperationIntelligence.Core.PagedResponse<SessionResponse>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                Items = items
            };
        }

        public async Task RevokeSessionAsync(Guid userId, Guid sessionId, CancellationToken cancellationToken = default)
        {
            var session = await _userSessionRepository.GetByIdWithUserAsync(sessionId, cancellationToken);
            if (session == null || session.UserId != userId)
                throw new KeyNotFoundException("Session not found.");

            await _userSessionRepository.RevokeSessionAsync(sessionId, cancellationToken);
            await _userSessionRepository.SaveChangesAsync(cancellationToken);
        }
    }
}
