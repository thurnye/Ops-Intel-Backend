namespace OperationIntelligence.DB;
public interface ICapacityReservationRepository
{
    Task<CapacityReservation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CapacityReservation>> GetByOperationIdAsync(Guid scheduleOperationId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CapacityReservation>> GetByResourceAsync(Guid resourceId, ResourceType resourceType, DateTime? startUtc = null, DateTime? endUtc = null, CancellationToken cancellationToken = default);

    Task AddAsync(CapacityReservation entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(CapacityReservation entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(CapacityReservation entity, CancellationToken cancellationToken = default);

    Task<bool> HasOverlapAsync(
        Guid resourceId,
        ResourceType resourceType,
        DateTime reservedStartUtc,
        DateTime reservedEndUtc,
        Guid? excludeReservationId = null,
        CancellationToken cancellationToken = default);
}
