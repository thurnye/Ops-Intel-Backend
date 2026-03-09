using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class ScheduleAuditLogRepository : IScheduleAuditLogRepository
{
    private readonly OperationIntelligenceDbContext _context;

    public ScheduleAuditLogRepository(OperationIntelligenceDbContext context)
    {
        _context = context;
    }

    public async Task<ScheduleAuditLog?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ScheduleAuditLogs
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }

    public async Task<(IReadOnlyList<ScheduleAuditLog> Items, int TotalRecords)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        pageNumber = pageNumber <= 0 ? 1 : pageNumber;
        pageSize = pageSize <= 0 ? 20 : pageSize;

        var query = _context.ScheduleAuditLogs
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .OrderByDescending(x => x.PerformedAtUtc);

        var totalRecords = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalRecords);
    }

    public async Task AddAsync(ScheduleAuditLog entity, CancellationToken cancellationToken = default)
    {
        await _context.ScheduleAuditLogs.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
