namespace OperationIntelligence.Core;

public interface IDashboardService
{
    Task<DashboardOverviewViewResponse> GetOverviewAsync(
        DashboardFilterRequest request,
        CancellationToken cancellationToken = default);
}