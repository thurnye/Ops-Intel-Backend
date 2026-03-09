using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class ShipmentAddressRepository : BaseRepository<ShipmentAddress>, IShipmentAddressRepository
{
    public ShipmentAddressRepository(OperationIntelligenceDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<ShipmentAddress>> SearchAsync(
        string? search = null,
        string? country = null,
        string? city = null,
        int take = 25,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();

            query = query.Where(x =>
                x.ContactName.Contains(term) ||
                (x.CompanyName != null && x.CompanyName.Contains(term)) ||
                x.AddressLine1.Contains(term) ||
                (x.AddressLine2 != null && x.AddressLine2.Contains(term)) ||
                x.City.Contains(term) ||
                x.StateOrProvince.Contains(term) ||
                x.PostalCode.Contains(term) ||
                x.Country.Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(country))
        {
            query = query.Where(x => x.Country == country);
        }

        if (!string.IsNullOrWhiteSpace(city))
        {
            query = query.Where(x => x.City == city);
        }

        return await query
            .OrderBy(x => x.ContactName)
            .ThenBy(x => x.AddressLine1)
            .Take(take)
            .ToListAsync(cancellationToken);
    }
}
