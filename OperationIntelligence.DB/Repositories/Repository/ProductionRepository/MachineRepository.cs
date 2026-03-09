using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class MachineRepository : BaseRepository<Machine>, IMachineRepository
{
    public MachineRepository(OperationIntelligenceDbContext context) : base(context)
    {
    }

    public async Task<Machine?> GetWithWorkCenterAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(x => x.WorkCenter)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }

    public async Task<IReadOnlyList<Machine>> GetActiveByWorkCenterIdAsync(Guid workCenterId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(x => x.WorkCenterId == workCenterId && x.IsActive && !x.IsDeleted)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Machine>> GetByStatusAsync(MachineStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(x => x.Status == status && !x.IsDeleted)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> MachineCodeExistsAsync(string machineCode, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(
            x => x.MachineCode == machineCode &&
                 !x.IsDeleted &&
                 (!excludeId.HasValue || x.Id != excludeId.Value),
            cancellationToken);
    }
}
