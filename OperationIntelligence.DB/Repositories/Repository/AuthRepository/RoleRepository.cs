using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class RoleRepository : BaseRepository<Role>, IRoleRepository
{
    public RoleRepository(OperationIntelligenceDbContext context) : base(context)
    {
    }

    public async Task<Role?> GetByNameAsync(string normalizedName, CancellationToken cancellationToken = default)
    {
        return await _context.Roles
            .FirstOrDefaultAsync(x => x.NormalizedName == normalizedName, cancellationToken);
    }

    public async Task<Role?> GetWithPermissionsAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        return await _context.Roles
            .Include(x => x.RolePermissions)
                .ThenInclude(x => x.Permission)
            .FirstOrDefaultAsync(x => x.Id == roleId, cancellationToken);
    }

    public async Task<IReadOnlyList<Role>> GetSystemRolesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Roles
            .AsNoTracking()
            .Where(x => x.IsSystemRole)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> RoleExistsAsync(string normalizedName, CancellationToken cancellationToken = default)
    {
        return await _context.Roles
            .AnyAsync(x => x.NormalizedName == normalizedName, cancellationToken);
    }
}
