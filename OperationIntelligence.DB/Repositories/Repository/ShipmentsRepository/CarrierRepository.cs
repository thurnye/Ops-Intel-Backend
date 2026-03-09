using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class CarrierRepository : BaseRepository<Carrier>, ICarrierRepository
{
    public CarrierRepository(OperationIntelligenceDbContext context) : base(context)
    {
    }

    public async Task<Carrier?> GetByIdWithServicesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(x => x.Services)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Carrier?> GetByCodeAsync(string carrierCode, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.CarrierCode == carrierCode, cancellationToken);
    }

    public async Task<IReadOnlyList<Carrier>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? search = null,
        bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        var query = BuildCarrierFilterQuery(_dbSet.AsNoTracking(), search, isActive);

        return await query
            .OrderBy(x => x.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(
        string? search = null,
        bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        var query = BuildCarrierFilterQuery(_dbSet.AsNoTracking(), search, isActive);
        return await query.CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Carrier>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<CarrierService?> GetServiceByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.CarrierServices
            .AsNoTracking()
            .Include(x => x.Carrier)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<CarrierService?> GetServiceByCodeAsync(Guid carrierId, string serviceCode, CancellationToken cancellationToken = default)
    {
        return await _context.CarrierServices
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.CarrierId == carrierId && x.ServiceCode == serviceCode, cancellationToken);
    }

    public async Task<IReadOnlyList<CarrierService>> GetServicesByCarrierIdAsync(Guid carrierId, bool? isActive = null, CancellationToken cancellationToken = default)
    {
        var query = _context.CarrierServices
            .AsNoTracking()
            .Where(x => x.CarrierId == carrierId);

        if (isActive.HasValue)
        {
            query = query.Where(x => x.IsActive == isActive.Value);
        }

        return await query
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    private static IQueryable<Carrier> BuildCarrierFilterQuery(
        IQueryable<Carrier> query,
        string? search,
        bool? isActive)
    {
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();

            query = query.Where(x =>
                x.Name.Contains(term) ||
                x.CarrierCode.Contains(term) ||
                (x.ContactName != null && x.ContactName.Contains(term)) ||
                (x.Email != null && x.Email.Contains(term)));
        }

        if (isActive.HasValue)
        {
            query = query.Where(x => x.IsActive == isActive.Value);
        }

        return query;
    }

    public async Task AddServiceAsync(CarrierService service, CancellationToken cancellationToken = default)
    => await _context.CarrierServices.AddAsync(service, cancellationToken);

    public void UpdateService(CarrierService service)
        => _context.CarrierServices.Update(service);

    public void RemoveService(CarrierService service)
        => _context.CarrierServices.Remove(service);
}
