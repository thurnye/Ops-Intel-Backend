using OperationIntelligence.Core.Models.Scheduling.Requests.Revision;
using OperationIntelligence.Core.Models.Scheduling.Responses.Revision;
using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class ScheduleRevisionService : IScheduleRevisionService
{
    private readonly IScheduleRevisionRepository _scheduleRevisionRepository;
    private readonly IScheduleRescheduleHistoryRepository _scheduleRescheduleHistoryRepository;
    private readonly IScheduleStatusHistoryRepository _scheduleStatusHistoryRepository;

    public ScheduleRevisionService(
        IScheduleRevisionRepository scheduleRevisionRepository,
        IScheduleRescheduleHistoryRepository scheduleRescheduleHistoryRepository,
        IScheduleStatusHistoryRepository scheduleStatusHistoryRepository)
    {
        _scheduleRevisionRepository = scheduleRevisionRepository;
        _scheduleRescheduleHistoryRepository = scheduleRescheduleHistoryRepository;
        _scheduleStatusHistoryRepository = scheduleStatusHistoryRepository;
    }

    public async Task<ScheduleRevisionResponse> CreateRevisionAsync(CreateScheduleRevisionRequest request, CancellationToken cancellationToken = default)
    {
        if (await _scheduleRevisionRepository.ExistsRevisionNumberAsync(request.SchedulePlanId, request.RevisionNo, cancellationToken))
            throw new InvalidOperationException(SchedulingErrorMessages.RevisionNumberAlreadyExistsForSchedulePlan);

        var entity = new ScheduleRevision
        {
            SchedulePlanId = request.SchedulePlanId,
            RevisionNo = request.RevisionNo,
            RevisionType = request.RevisionType.Trim(),
            ChangeSummary = request.ChangeSummary.Trim(),
            Reason = request.Reason.Trim(),
            RevisedAtUtc = request.RevisedAtUtc,
            SnapshotJson = request.SnapshotJson,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        await _scheduleRevisionRepository.AddAsync(entity, cancellationToken);

        return new ScheduleRevisionResponse
        {
            Id = entity.Id,
            SchedulePlanId = entity.SchedulePlanId,
            RevisionNo = entity.RevisionNo,
            RevisionType = entity.RevisionType,
            ChangeSummary = entity.ChangeSummary,
            Reason = entity.Reason,
            RevisedAtUtc = entity.RevisedAtUtc,
            SnapshotJson = entity.SnapshotJson
        };
    }

    public async Task<ScheduleRescheduleHistoryResponse> CreateRescheduleHistoryAsync(CreateScheduleRescheduleHistoryRequest request, CancellationToken cancellationToken = default)
    {
        var entity = new ScheduleRescheduleHistory
        {
            SchedulePlanId = request.SchedulePlanId,
            ScheduleJobId = request.ScheduleJobId,
            ScheduleOperationId = request.ScheduleOperationId,
            OldPlannedStartUtc = request.OldPlannedStartUtc,
            OldPlannedEndUtc = request.OldPlannedEndUtc,
            NewPlannedStartUtc = request.NewPlannedStartUtc,
            NewPlannedEndUtc = request.NewPlannedEndUtc,
            OldWorkCenterId = request.OldWorkCenterId,
            NewWorkCenterId = request.NewWorkCenterId,
            OldMachineId = request.OldMachineId,
            NewMachineId = request.NewMachineId,
            ReasonCode = request.ReasonCode.Trim(),
            ReasonDescription = request.ReasonDescription.Trim(),
            ChangedAtUtc = request.ChangedAtUtc,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        await _scheduleRescheduleHistoryRepository.AddAsync(entity, cancellationToken);

        return new ScheduleRescheduleHistoryResponse
        {
            Id = entity.Id,
            SchedulePlanId = entity.SchedulePlanId,
            ScheduleJobId = entity.ScheduleJobId,
            ScheduleOperationId = entity.ScheduleOperationId,
            OldPlannedStartUtc = entity.OldPlannedStartUtc,
            OldPlannedEndUtc = entity.OldPlannedEndUtc,
            NewPlannedStartUtc = entity.NewPlannedStartUtc,
            NewPlannedEndUtc = entity.NewPlannedEndUtc,
            OldWorkCenterId = entity.OldWorkCenterId,
            OldWorkCenterName = entity.OldWorkCenter?.Name,
            NewWorkCenterId = entity.NewWorkCenterId,
            NewWorkCenterName = entity.NewWorkCenter?.Name,
            OldMachineId = entity.OldMachineId,
            OldMachineName = entity.OldMachine?.Name,
            NewMachineId = entity.NewMachineId,
            NewMachineName = entity.NewMachine?.Name,
            ReasonCode = entity.ReasonCode,
            ReasonDescription = entity.ReasonDescription,
            ChangedAtUtc = entity.ChangedAtUtc
        };
    }

    public async Task<ScheduleStatusHistoryResponse> CreateStatusHistoryAsync(CreateScheduleStatusHistoryRequest request, CancellationToken cancellationToken = default)
    {
        var entity = new ScheduleStatusHistory
        {
            SchedulePlanId = request.SchedulePlanId,
            ScheduleJobId = request.ScheduleJobId,
            ScheduleOperationId = request.ScheduleOperationId,
            EntityType = request.EntityType.Trim(),
            OldStatus = request.OldStatus.Trim(),
            NewStatus = request.NewStatus.Trim(),
            Reason = request.Reason?.Trim(),
            Notes = request.Notes?.Trim(),
            ChangedAtUtc = request.ChangedAtUtc,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        await _scheduleStatusHistoryRepository.AddAsync(entity, cancellationToken);

        return new ScheduleStatusHistoryResponse
        {
            Id = entity.Id,
            SchedulePlanId = entity.SchedulePlanId,
            ScheduleJobId = entity.ScheduleJobId,
            ScheduleOperationId = entity.ScheduleOperationId,
            EntityType = entity.EntityType,
            OldStatus = entity.OldStatus,
            NewStatus = entity.NewStatus,
            Reason = entity.Reason,
            Notes = entity.Notes,
            ChangedAtUtc = entity.ChangedAtUtc
        };
    }

    public async Task<ScheduleRevisionResponse?> GetRevisionByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _scheduleRevisionRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return null;

        return new ScheduleRevisionResponse
        {
            Id = entity.Id,
            SchedulePlanId = entity.SchedulePlanId,
            RevisionNo = entity.RevisionNo,
            RevisionType = entity.RevisionType,
            ChangeSummary = entity.ChangeSummary,
            Reason = entity.Reason,
            RevisedAtUtc = entity.RevisedAtUtc,
            SnapshotJson = entity.SnapshotJson
        };
    }

    public async Task<ScheduleRescheduleHistoryResponse?> GetRescheduleHistoryByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _scheduleRescheduleHistoryRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return null;

        return new ScheduleRescheduleHistoryResponse
        {
            Id = entity.Id,
            SchedulePlanId = entity.SchedulePlanId,
            ScheduleJobId = entity.ScheduleJobId,
            ScheduleOperationId = entity.ScheduleOperationId,
            OldPlannedStartUtc = entity.OldPlannedStartUtc,
            OldPlannedEndUtc = entity.OldPlannedEndUtc,
            NewPlannedStartUtc = entity.NewPlannedStartUtc,
            NewPlannedEndUtc = entity.NewPlannedEndUtc,
            OldWorkCenterId = entity.OldWorkCenterId,
            OldWorkCenterName = entity.OldWorkCenter?.Name,
            NewWorkCenterId = entity.NewWorkCenterId,
            NewWorkCenterName = entity.NewWorkCenter?.Name,
            OldMachineId = entity.OldMachineId,
            OldMachineName = entity.OldMachine?.Name,
            NewMachineId = entity.NewMachineId,
            NewMachineName = entity.NewMachine?.Name,
            ReasonCode = entity.ReasonCode,
            ReasonDescription = entity.ReasonDescription,
            ChangedAtUtc = entity.ChangedAtUtc
        };
    }

    public async Task<ScheduleStatusHistoryResponse?> GetStatusHistoryByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _scheduleStatusHistoryRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return null;

        return new ScheduleStatusHistoryResponse
        {
            Id = entity.Id,
            SchedulePlanId = entity.SchedulePlanId,
            ScheduleJobId = entity.ScheduleJobId,
            ScheduleOperationId = entity.ScheduleOperationId,
            EntityType = entity.EntityType,
            OldStatus = entity.OldStatus,
            NewStatus = entity.NewStatus,
            Reason = entity.Reason,
            Notes = entity.Notes,
            ChangedAtUtc = entity.ChangedAtUtc
        };
    }
}
