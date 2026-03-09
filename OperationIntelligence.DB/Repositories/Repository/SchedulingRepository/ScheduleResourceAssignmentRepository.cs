using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class ScheduleResourceAssignmentRepository : IScheduleResourceAssignmentRepository
{
    private readonly OperationIntelligenceDbContext _context;

    public ScheduleResourceAssignmentRepository(OperationIntelligenceDbContext context)
    {
        _context = context;
    }

    public async Task<ScheduleResourceAssignment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ScheduleResourceAssignments
            .AsNoTracking()
            .Include(x => x.Shift)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }

    public async Task<IReadOnlyList<ScheduleResourceAssignment>> GetByOperationIdAsync(Guid operationId, CancellationToken cancellationToken = default)
    {
        return await _context.ScheduleResourceAssignments
            .AsNoTracking()
            .Include(x => x.Shift)
            .Where(x => x.ScheduleOperationId == operationId && !x.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(ScheduleResourceAssignment entity, CancellationToken cancellationToken = default)
    {
        await _context.ScheduleResourceAssignments.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(ScheduleResourceAssignment entity, CancellationToken cancellationToken = default)
    {
        _context.ScheduleResourceAssignments.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(ScheduleResourceAssignment entity, CancellationToken cancellationToken = default)
    {
        _context.ScheduleResourceAssignments.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> HasOverlappingAssignmentAsync(
        Guid resourceId,
        ResourceType resourceType,
        DateTime assignedStartUtc,
        DateTime assignedEndUtc,
        Guid? excludeAssignmentId = null,
        CancellationToken cancellationToken = default)
    {
        return await _context.ScheduleResourceAssignments.AnyAsync(x =>
            x.ResourceId == resourceId &&
            x.ResourceType == resourceType &&
            !x.IsDeleted &&
            (!excludeAssignmentId.HasValue || x.Id != excludeAssignmentId.Value) &&
            x.AssignedStartUtc < assignedEndUtc &&
            x.AssignedEndUtc > assignedStartUtc,
            cancellationToken);
    }
}
