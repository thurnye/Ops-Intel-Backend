using OperationIntelligence.DB;

namespace OperationIntelligence.Core
{
    public static class AuthMappingExtensions
    {
        public static UserResponse ToUserResponse(this PlatformUser user)
        {
            var roles = user.UserRoles
                .Select(ur => ur.Role?.Name)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Cast<string>()
                .Distinct()
                .ToList();

            var permissions = user.UserRoles
                .SelectMany(ur => ur.Role?.RolePermissions ?? Enumerable.Empty<RolePermission>())
                .Select(rp => rp.Permission?.Code)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Cast<string>()
                .Distinct()
                .ToList();

            return new UserResponse
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                TwoFactorEnabled = user.TwoFactorEnabled,
                IsActive = user.IsActive,
                IsLocked = user.IsLocked,
                LastLoginAtUtc = user.LastLoginAtUtc,
                Roles = roles,
                Permissions = permissions,
                Profile = new UserProfileResponse
                {
                    FirstName = user.Profile?.FirstName ?? string.Empty,
                    LastName = user.Profile?.LastName ?? string.Empty,
                    DisplayName = user.Profile?.DisplayName,
                    Birthdate = user.Profile?.Birthdate,
                    Gender = user.Profile?.Gender,
                    PhoneNumber = user.Profile?.PhoneNumber,
                    AddressLine1 = user.Profile?.AddressLine1,
                    AddressLine2 = user.Profile?.AddressLine2,
                    City = user.Profile?.City,
                    StateOrProvince = user.Profile?.StateOrProvince,
                    Country = user.Profile?.Country,
                    PostalCode = user.Profile?.PostalCode,
                    AvatarFileId = user.Profile?.AvatarFileId,
                    AvatarUrl = user.Profile?.AvatarFile?.PublicUrl
                }
            };
        }

        public static SessionResponse ToSessionResponse(this UserSession session)
        {
            return new SessionResponse
            {
                Id = session.Id,
                IpAddress = session.IpAddress,
                UserAgent = session.UserAgent,
                DeviceName = session.DeviceName,
                Browser = session.Browser,
                OperatingSystem = session.OperatingSystem,
                CreatedAtUtc = session.CreatedAtUtc,
                LastSeenAtUtc = session.LastSeenAtUtc,
                RevokedAtUtc = session.RevokedAtUtc
            };
        }
    }
}
