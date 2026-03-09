namespace OperationIntelligence.DB;

public interface ICarrierRepository : IBaseRepository<Carrier>
{
    Task<Carrier?> GetByIdWithServicesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Carrier?> GetByCodeAsync(string carrierCode, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Carrier>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? search = null,
        bool? isActive = null,
        CancellationToken cancellationToken = default);

    Task<int> CountAsync(
        string? search = null,
        bool? isActive = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Carrier>> GetActiveAsync(CancellationToken cancellationToken = default);

    Task<CarrierService?> GetServiceByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CarrierService?> GetServiceByCodeAsync(Guid carrierId, string serviceCode, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CarrierService>> GetServicesByCarrierIdAsync(Guid carrierId, bool? isActive = null, CancellationToken cancellationToken = default);

    Task AddServiceAsync(CarrierService service, CancellationToken cancellationToken = default);
    void UpdateService(CarrierService service);
    void RemoveService(CarrierService service);
}