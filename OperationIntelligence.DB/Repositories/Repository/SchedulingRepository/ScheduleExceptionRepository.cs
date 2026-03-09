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
        CancellationToken cancellationToken = default)
    {
        pageNumber = pageNumber <= 0 ? 1 : pageNumber;
        pageSize = pageSize <= 0 ? 20 : pageSize;

        var query = _context.ScheduleExceptions
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .OrderByDescending(x => x.DetectedAtUtc);

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
