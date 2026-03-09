namespace OperationIntelligence.DB;

public interface IDockAppointmentRepository : IBaseRepository<DockAppointment>
{
    Task<DockAppointment?> GetByIdWithShipmentsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<DockAppointment?> GetByAppointmentNumberAsync(string appointmentNumber, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DockAppointment>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? search = null,
        DockAppointmentStatus? status = null,
        Guid? warehouseId = null,
        Guid? carrierId = null,
        DateTime? scheduledStartFromUtc = null,
        DateTime? scheduledStartToUtc = null,
        CancellationToken cancellationToken = default);

    Task<int> CountAsync(
        string? search = null,
        DockAppointmentStatus? status = null,
        Guid? warehouseId = null,
        Guid? carrierId = null,
        DateTime? scheduledStartFromUtc = null,
        DateTime? scheduledStartToUtc = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DockAppointment>> GetUpcomingAsync(
        Guid? warehouseId = null,
        DateTime? fromUtc = null,
        CancellationToken cancellationToken = default);

    Task<bool> HasDockConflictAsync(
        Guid warehouseId,
        string dockCode,
        DateTime scheduledStartUtc,
        DateTime scheduledEndUtc,
        Guid? excludeAppointmentId = null,
        CancellationToken cancellationToken = default);
}
