namespace OperationIntelligence.DB;

public interface IMachineRepository : IBaseRepository<Machine>
{
    Task<Machine?> GetWithWorkCenterAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Machine>> GetActiveByWorkCenterIdAsync(Guid workCenterId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Machine>> GetByStatusAsync(MachineStatus status, CancellationToken cancellationToken = default);

    Task<bool> MachineCodeExistsAsync(string machineCode, Guid? excludeId = null, CancellationToken cancellationToken = default);
}
