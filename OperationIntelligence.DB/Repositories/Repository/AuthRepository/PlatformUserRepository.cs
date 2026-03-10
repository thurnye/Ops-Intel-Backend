using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class PlatformUserRepository : BaseRepository<PlatformUser>, IPlatformUserRepository
{
    public PlatformUserRepository(OperationIntelligenceDbContext context) : base(context)
    {
    }

    public async Task<PlatformUser?> GetByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Include(x => x.Profile)
                .ThenInclude(x => x!.AvatarFile)
            .FirstOrDefaultAsync(x => x.NormalizedEmail == normalizedEmail, cancellationToken);
    }

    public async Task<PlatformUser?> GetByUserNameAsync(string normalizedUserName, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Include(x => x.Profile)
                .ThenInclude(x => x!.AvatarFile)
            .FirstOrDefaultAsync(x => x.NormalizedUserName == normalizedUserName, cancellationToken);
    }

    public async Task<PlatformUser?> GetByEmailOrUserNameAsync(string normalizedValue, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Include(x => x.Profile)
                .ThenInclude(x => x!.AvatarFile)
            .FirstOrDefaultAsync(
                x => x.NormalizedEmail == normalizedValue || x.NormalizedUserName == normalizedValue,
                cancellationToken);
    }

    public async Task<PlatformUser?> GetWithProfileAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Include(x => x.Profile)
                .ThenInclude(x => x!.AvatarFile)
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
    }

    public async Task<PlatformUser?> GetWithRolesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
    }

    public async Task<PlatformUser?> GetFullAuthUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Include(x => x.Profile)
                .ThenInclude(x => x!.AvatarFile)
            .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
                    .ThenInclude(x => x.RolePermissions)
                        .ThenInclude(x => x.Permission)
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string normalizedEmail, CancellationToken cancellationToken = default)
    {
        return await _context.Users.AnyAsync(x => x.NormalizedEmail == normalizedEmail, cancellationToken);
    }

    public async Task<bool> UserNameExistsAsync(string normalizedUserName, CancellationToken cancellationToken = default)
    {
        return await _context.Users.AnyAsync(x => x.NormalizedUserName == normalizedUserName, cancellationToken);
    }

    public async Task AddUserWithProfileAsync(
        PlatformUser user,
        PlatformUserProfile profile,
        CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(user, cancellationToken);

        profile.UserId = user.Id;
        await _context.UserProfiles.AddAsync(profile, cancellationToken);
    }
}
