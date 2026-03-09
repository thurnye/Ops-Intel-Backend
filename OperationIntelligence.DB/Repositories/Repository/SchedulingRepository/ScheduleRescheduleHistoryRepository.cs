using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class ScheduleRescheduleHistoryRepository : IScheduleRescheduleHistoryRepository
{
    private readonly OperationIntelligenceDbContext _context;

    public ScheduleRescheduleHistoryRepository(OperationIntelligenceDbContext context)
    {
        _context = context;
    }

    public async Task<ScheduleRescheduleHistory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ScheduleRescheduleHistories
            .AsNoTracking()
            .Include(x => x.OldWorkCenter)
            .Include(x => x.NewWorkCenter)
            .Include(x => x.OldMachine)
            .Include(x => x.NewMachine)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }

    public async Task<IReadOnlyList<ScheduleRescheduleHistory>> GetBySchedulePlanIdAsync(Guid schedulePlanId, CancellationToken cancellationToken = default)
    {
        return await _context.ScheduleRescheduleHistories
            .AsNoTracking()
            .Where(x => x.SchedulePlanId == schedulePlanId && !x.IsDeleted)
            .OrderByDescending(x => x.ChangedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ScheduleRescheduleHistory>> GetByScheduleJobIdAsync(Guid scheduleJobId, CancellationToken cancellationToken = default)
    {
        return await _context.ScheduleRescheduleHistories
            .AsNoTracking()
            .Where(x => x.ScheduleJobId == scheduleJobId && !x.IsDeleted)
            .OrderByDescending(x => x.ChangedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ScheduleRescheduleHistory>> GetByScheduleOperationIdAsync(Guid scheduleOperationId, CancellationToken cancellationToken = default)
    {
        return await _context.ScheduleRescheduleHistories
            .AsNoTracking()
            .Where(x => x.ScheduleOperationId == scheduleOperationId && !x.IsDeleted)
            .OrderByDescending(x => x.ChangedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(ScheduleRescheduleHistory entity, CancellationToken cancellationToken = default)
    {
        await _context.ScheduleRescheduleHistories.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
