using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class ScheduleStatusHistoryRepository : IScheduleStatusHistoryRepository
{
    private readonly OperationIntelligenceDbContext _context;

    public ScheduleStatusHistoryRepository(OperationIntelligenceDbContext context)
    {
        _context = context;
    }

    public async Task<ScheduleStatusHistory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ScheduleStatusHistories
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }

    public async Task<IReadOnlyList<ScheduleStatusHistory>> GetBySchedulePlanIdAsync(Guid schedulePlanId, CancellationToken cancellationToken = default)
    {
        return await _context.ScheduleStatusHistories
            .AsNoTracking()
            .Where(x => x.SchedulePlanId == schedulePlanId && !x.IsDeleted)
            .OrderByDescending(x => x.ChangedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ScheduleStatusHistory>> GetByScheduleJobIdAsync(Guid scheduleJobId, CancellationToken cancellationToken = default)
    {
        return await _context.ScheduleStatusHistories
            .AsNoTracking()
            .Where(x => x.ScheduleJobId == scheduleJobId && !x.IsDeleted)
            .OrderByDescending(x => x.ChangedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ScheduleStatusHistory>> GetByScheduleOperationIdAsync(Guid scheduleOperationId, CancellationToken cancellationToken = default)
    {
        return await _context.ScheduleStatusHistories
            .AsNoTracking()
            .Where(x => x.ScheduleOperationId == scheduleOperationId && !x.IsDeleted)
            .OrderByDescending(x => x.ChangedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(ScheduleStatusHistory entity, CancellationToken cancellationToken = default)
    {
        await _context.ScheduleStatusHistories.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
