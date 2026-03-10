using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class PermissionRepository : BaseRepository<Permission>, IPermissionRepository
{
    public PermissionRepository(OperationIntelligenceDbContext context) : base(context)
    {
    }

    public async Task<Permission?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _context.Permissions
            .FirstOrDefaultAsync(x => x.Code == code, cancellationToken);
    }

    public async Task<IReadOnlyList<Permission>> GetByCodesAsync(IEnumerable<string> codes, CancellationToken cancellationToken = default)
    {
        var codeList = codes.Distinct().ToList();

        return await _context.Permissions
            .AsNoTracking()
            .Where(x => codeList.Contains(x.Code))
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> PermissionExistsAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _context.Permissions
            .AnyAsync(x => x.Code == code, cancellationToken);
    }
}
