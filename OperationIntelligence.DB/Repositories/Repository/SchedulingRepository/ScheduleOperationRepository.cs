using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class ScheduleOperationRepository : IScheduleOperationRepository
{
    private readonly OperationIntelligenceDbContext _context;

    public ScheduleOperationRepository(OperationIntelligenceDbContext context)
    {
        _context = context;
    }

    public async Task<ScheduleOperation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ScheduleOperations
            .AsNoTracking()
            .Include(x => x.ScheduleJob)
            .Include(x => x.RoutingStep)
            .Include(x => x.WorkCenter)
            .Include(x => x.Machine)
            .Include(x => x.PlannedShift)
            .Include(x => x.ActualShift)
            .Include(x => x.ProductionExecution)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }

    public async Task<ScheduleOperation?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ScheduleOperations
            .AsNoTracking()
            .Include(x => x.ScheduleJob)
            .Include(x => x.RoutingStep)
            .Include(x => x.WorkCenter)
            .Include(x => x.Machine)
            .Include(x => x.PlannedShift)
            .Include(x => x.ActualShift)
            .Include(x => x.ProductionExecution)
            .Include(x => x.PredecessorDependencies)
                .ThenInclude(x => x.PredecessorOperation)
            .Include(x => x.SuccessorDependencies)
                .ThenInclude(x => x.SuccessorOperation)
            .Include(x => x.Constraints)
            .Include(x => x.ResourceOptions)
            .Include(x => x.ResourceAssignments)
                .ThenInclude(x => x.Shift)
            .Include(x => x.CapacityReservations)
                .ThenInclude(x => x.Shift)
            .Include(x => x.DispatchQueueItems)
            .Include(x => x.MaterialChecks)
            .Include(x => x.ScheduleExceptions)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }

    public async Task<(IReadOnlyList<ScheduleOperation> Items, int TotalRecords)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        pageNumber = pageNumber <= 0 ? 1 : pageNumber;
        pageSize = pageSize <= 0 ? 20 : pageSize;

        var query = _context.ScheduleOperations
            .AsNoTracking()
            .Include(x => x.ScheduleJob)
            .Include(x => x.RoutingStep)
            .Include(x => x.WorkCenter)
            .Include(x => x.Machine)
            .Include(x => x.PlannedShift)
            .Include(x => x.ActualShift)
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.PlannedStartUtc);

        var totalRecords = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalRecords);
    }

    public async Task<IReadOnlyList<ScheduleOperation>> GetByScheduleJobIdAsync(Guid scheduleJobId, CancellationToken cancellationToken = default)
    {
        return await _context.ScheduleOperations
            .AsNoTracking()
            .Where(x => x.ScheduleJobId == scheduleJobId && !x.IsDeleted)
            .OrderBy(x => x.SequenceNo)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ScheduleOperation>> GetByWorkCenterAsync(Guid workCenterId, DateTime? startUtc = null, DateTime? endUtc = null, CancellationToken cancellationToken = default)
    {
        var query = _context.ScheduleOperations
            .AsNoTracking()
            .Where(x => x.WorkCenterId == workCenterId && !x.IsDeleted);

        if (startUtc.HasValue)
            query = query.Where(x => x.PlannedEndUtc >= startUtc.Value);

        if (endUtc.HasValue)
            query = query.Where(x => x.PlannedStartUtc <= endUtc.Value);

        return await query.OrderBy(x => x.PlannedStartUtc).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ScheduleOperation>> GetByMachineAsync(Guid machineId, DateTime? startUtc = null, DateTime? endUtc = null, CancellationToken cancellationToken = default)
    {
        var query = _context.ScheduleOperations
            .AsNoTracking()
            .Where(x => x.MachineId == machineId && !x.IsDeleted);

        if (startUtc.HasValue)
            query = query.Where(x => x.PlannedEndUtc >= startUtc.Value);

        if (endUtc.HasValue)
            query = query.Where(x => x.PlannedStartUtc <= endUtc.Value);

        return await query.OrderBy(x => x.PlannedStartUtc).ToListAsync(cancellationToken);
    }

    public async Task AddAsync(ScheduleOperation entity, CancellationToken cancellationToken = default)
    {
        await _context.ScheduleOperations.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(ScheduleOperation entity, CancellationToken cancellationToken = default)
    {
        _context.ScheduleOperations.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(ScheduleOperation entity, CancellationToken cancellationToken = default)
    {
        _context.ScheduleOperations.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> HasOverlappingOperationOnWorkCenterAsync(Guid workCenterId, DateTime plannedStartUtc, DateTime plannedEndUtc, Guid? excludeOperationId = null, CancellationToken cancellationToken = default)
    {
        return await _context.ScheduleOperations.AnyAsync(x =>
            x.WorkCenterId == workCenterId &&
            !x.IsDeleted &&
            (!excludeOperationId.HasValue || x.Id != excludeOperationId.Value) &&
            x.PlannedStartUtc < plannedEndUtc &&
            x.PlannedEndUtc > plannedStartUtc,
            cancellationToken);
    }

    public async Task<bool> HasOverlappingOperationOnMachineAsync(Guid machineId, DateTime plannedStartUtc, DateTime plannedEndUtc, Guid? excludeOperationId = null, CancellationToken cancellationToken = default)
    {
        return await _context.ScheduleOperations.AnyAsync(x =>
            x.MachineId == machineId &&
            !x.IsDeleted &&
            (!excludeOperationId.HasValue || x.Id != excludeOperationId.Value) &&
            x.PlannedStartUtc < plannedEndUtc &&
            x.PlannedEndUtc > plannedStartUtc,
            cancellationToken);
    }
}
