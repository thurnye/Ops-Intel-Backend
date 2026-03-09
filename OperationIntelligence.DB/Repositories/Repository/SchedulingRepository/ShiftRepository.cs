using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class ShiftRepository : IShiftRepository
{
    private readonly OperationIntelligenceDbContext _context;

    public ShiftRepository(OperationIntelligenceDbContext context)
    {
        _context = context;
    }

    public async Task<Shift?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Shifts
            .AsNoTracking()
            .Include(x => x.Warehouse)
            .Include(x => x.WorkCenter)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }

    public async Task<Shift?> GetByCodeAsync(Guid warehouseId, string shiftCode, CancellationToken cancellationToken = default)
    {
        return await _context.Shifts
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.WarehouseId == warehouseId && x.ShiftCode == shiftCode && !x.IsDeleted, cancellationToken);
    }

    public async Task<(IReadOnlyList<Shift> Items, int TotalRecords)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        pageNumber = pageNumber <= 0 ? 1 : pageNumber;
        pageSize = pageSize <= 0 ? 20 : pageSize;

        var query = _context.Shifts
            .AsNoTracking()
            .Include(x => x.Warehouse)
            .Include(x => x.WorkCenter)
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.ShiftCode);

        var totalRecords = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalRecords);
    }

    public async Task<IReadOnlyList<Shift>> GetByWarehouseIdAsync(Guid warehouseId, CancellationToken cancellationToken = default)
    {
        return await _context.Shifts
            .AsNoTracking()
            .Where(x => x.WarehouseId == warehouseId && !x.IsDeleted)
            .OrderBy(x => x.ShiftCode)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Shift>> GetByWorkCenterIdAsync(Guid workCenterId, CancellationToken cancellationToken = default)
    {
        return await _context.Shifts
            .AsNoTracking()
            .Where(x => x.WorkCenterId == workCenterId && !x.IsDeleted)
            .OrderBy(x => x.ShiftCode)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Shift entity, CancellationToken cancellationToken = default)
    {
        await _context.Shifts.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Shift entity, CancellationToken cancellationToken = default)
    {
        _context.Shifts.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Shift entity, CancellationToken cancellationToken = default)
    {
        _context.Shifts.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsByCodeAsync(Guid warehouseId, string shiftCode, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        return await _context.Shifts.AnyAsync(x =>
            x.WarehouseId == warehouseId &&
            x.ShiftCode == shiftCode &&
            !x.IsDeleted &&
            (!excludeId.HasValue || x.Id != excludeId.Value),
            cancellationToken);
    }
}
