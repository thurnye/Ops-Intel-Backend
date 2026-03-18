namespace OperationIntelligence.DB;

public interface IDashboardReadRepository
{
    Task<DashboardOverviewReadModel> GetOverviewAsync(
        string range,
        string site,
        CancellationToken cancellationToken = default);
}
