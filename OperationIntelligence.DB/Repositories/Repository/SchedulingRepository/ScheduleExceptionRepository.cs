using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class ScheduleExceptionRepository : IScheduleExceptionRepository
{
    private readonly OperationIntelligenceDbContext _context;

    public ScheduleExceptionRepository(OperationIntelligenceDbContext context)
    {
        _context = context;
    }

    public async Task<ScheduleException?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ScheduleExceptions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }

    public async Task<(IReadOnlyList<ScheduleException> Items, int TotalRecords)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? search = null,
        DateTime? startDateUtc = null,
        DateTime? endDateUtc = null,
        Guid? schedulePlanId = null,
        Guid? scheduleJobId = null,
        Guid? scheduleOperationId = null,
        int? exceptionType = null,
        int? severity = null,
        int? status = null,
        string? assignedTo = null,
        CancellationToken cancellationToken = default)
    {
        pageNumber = pageNumber <= 0 ? 1 : pageNumber;
        pageSize = pageSize <= 0 ? 20 : pageSize;

        var query = _context.ScheduleExceptions
            .AsNoTracking()
            .Where(x => !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(x =>
                x.Title.Contains(term) ||
                x.Description.Contains(term) ||
                (x.AssignedTo != null && x.AssignedTo.Contains(term)));
        }

        if (startDateUtc.HasValue)
        {
            query = query.Where(x => x.DetectedAtUtc >= startDateUtc.Value);
        }

        if (endDateUtc.HasValue)
        {
            query = query.Where(x => x.DetectedAtUtc <= endDateUtc.Value);
        }

        if (schedulePlanId.HasValue)
        {
            query = query.Where(x => x.SchedulePlanId == schedulePlanId.Value);
        }

        if (scheduleJobId.HasValue)
        {
            query = query.Where(x => x.ScheduleJobId == scheduleJobId.Value);
        }

        if (scheduleOperationId.HasValue)
        {
            query = query.Where(x => x.ScheduleOperationId == scheduleOperationId.Value);
        }

        if (exceptionType.HasValue)
        {
            query = query.Where(x => (int)x.ExceptionType == exceptionType.Value);
        }

        if (severity.HasValue)
        {
            query = query.Where(x => (int)x.Severity == severity.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(x => (int)x.Status == status.Value);
        }

        if (!string.IsNullOrWhiteSpace(assignedTo))
        {
            var assignee = assignedTo.Trim();
            query = query.Where(x => x.AssignedTo != null && x.AssignedTo.Contains(assignee));
        }

        query = query.OrderByDescending(x => x.DetectedAtUtc);

        var totalRecords = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalRecords);
    }

    public async Task AddAsync(ScheduleException entity, CancellationToken cancellationToken = default)
    {
        await _context.ScheduleExceptions.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(ScheduleException entity, CancellationToken cancellationToken = default)
    {
        _context.ScheduleExceptions.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(ScheduleException entity, CancellationToken cancellationToken = default)
    {
        _context.ScheduleExceptions.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
