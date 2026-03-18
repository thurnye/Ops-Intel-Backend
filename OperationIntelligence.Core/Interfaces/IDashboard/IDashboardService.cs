namespace OperationIntelligence.Core;

public interface IDashboardService
{
    Task<DashboardOverviewResponse> GetOverviewAsync(
        DashboardFilterRequest request,
        CancellationToken cancellationToken = default);
}
