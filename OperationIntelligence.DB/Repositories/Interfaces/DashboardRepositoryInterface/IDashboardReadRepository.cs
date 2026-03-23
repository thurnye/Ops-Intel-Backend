namespace OperationIntelligence.DB;

public interface IDashboardReadRepository
{
    Task<DashboardOverviewReadModel> GetOverviewAsync(
        DateOnly? from,
        DateOnly? to,
        string site,
        CancellationToken cancellationToken = default);
}