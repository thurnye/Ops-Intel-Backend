using Microsoft.Extensions.Logging;
using OperationIntelligence.Core.Models.Scheduling.Requests.ScheduleOperation;
using OperationIntelligence.Core.Models.Scheduling.Responses.Capacity;
using OperationIntelligence.Core.Models.Scheduling.Responses.Dispatch;
using OperationIntelligence.Core.Models.Scheduling.Responses.ScheduleOperation;
using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class ScheduleOperationService : IScheduleOperationService
{
    private readonly IScheduleOperationRepository _scheduleOperationRepository;
    private readonly IScheduleOperationDependencyRepository _dependencyRepository;
    private readonly IScheduleOperationConstraintRepository _constraintRepository;
    private readonly IScheduleOperationResourceOptionRepository _resourceOptionRepository;
    private readonly IScheduleResourceAssignmentRepository _resourceAssignmentRepository;
    private readonly ILogger<ScheduleOperationService> _logger;

    public ScheduleOperationService(
        IScheduleOperationRepository scheduleOperationRepository,
        IScheduleOperationDependencyRepository dependencyRepository,
        IScheduleOperationConstraintRepository constraintRepository,
        IScheduleOperationResourceOptionRepository resourceOptionRepository,
        IScheduleResourceAssignmentRepository resourceAssignmentRepository,
        ILogger<ScheduleOperationService> logger)
    {
        _scheduleOperationRepository = scheduleOperationRepository;
        _dependencyRepository = dependencyRepository;
        _constraintRepository = constraintRepository;
        _resourceOptionRepository = resourceOptionRepository;
        _resourceAssignmentRepository = resourceAssignmentRepository;
        _logger = logger;
    }

    public async Task<ScheduleOperationResponse> CreateAsync(CreateScheduleOperationRequest request, CancellationToken cancellationToken = default)
    {
        if (await _scheduleOperationRepository.HasOverlappingOperationOnWorkCenterAsync(request.WorkCenterId, request.PlannedStartUtc, request.PlannedEndUtc, null, cancellationToken))
            throw new InvalidOperationException(SchedulingErrorMessages.OverlappingWorkCenterOperationDetected);

        if (request.MachineId.HasValue &&
            await _scheduleOperationRepository.HasOverlappingOperationOnMachineAsync(request.MachineId.Value, request.PlannedStartUtc, request.PlannedEndUtc, null, cancellationToken))
            throw new InvalidOperationException(SchedulingErrorMessages.OverlappingMachineOperationDetected);

        var entity = new ScheduleOperation
        {
            ScheduleJobId = request.ScheduleJobId,
            RoutingStepId = request.RoutingStepId,
            WorkCenterId = request.WorkCenterId,
            MachineId = request.MachineId,
            PlannedShiftId = request.PlannedShiftId,
            SequenceNo = request.SequenceNo,
            OperationCode = request.OperationCode.Trim(),
            OperationName = request.OperationName.Trim(),
            PlannedStartUtc = request.PlannedStartUtc,
            PlannedEndUtc = request.PlannedEndUtc,
            SetupTimeMinutes = request.SetupTimeMinutes,
            RunTimeMinutes = request.RunTimeMinutes,
            QueueTimeMinutes = request.QueueTimeMinutes,
            WaitTimeMinutes = request.WaitTimeMinutes,
            MoveTimeMinutes = request.MoveTimeMinutes,
            PlannedQuantity = request.PlannedQuantity,
            Status = ScheduleOperationStatus.Scheduled,
            DispatchStatus = DispatchStatus.NotDispatched,
            IsCriticalPath = request.IsCriticalPath,
            IsBottleneckOperation = request.IsBottleneckOperation,
            IsOutsourced = request.IsOutsourced,
            PriorityScore = request.PriorityScore,
            ConstraintReason = request.ConstraintReason?.Trim(),
            Notes = request.Notes?.Trim(),
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        await _scheduleOperationRepository.AddAsync(entity, cancellationToken);
        var created = await _scheduleOperationRepository.GetByIdAsync(entity.Id, cancellationToken) ?? entity;
        return MapToResponse(created);
    }

    public async Task<ScheduleOperationResponse> UpdateAsync(Guid id, UpdateScheduleOperationRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _scheduleOperationRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException(SchedulingErrorMessages.ScheduleOperationNotFound);

        if (await _scheduleOperationRepository.HasOverlappingOperationOnWorkCenterAsync(request.WorkCenterId, request.PlannedStartUtc, request.PlannedEndUtc, id, cancellationToken))
            throw new InvalidOperationException(SchedulingErrorMessages.OverlappingWorkCenterOperationDetected);

        if (request.MachineId.HasValue &&
            await _scheduleOperationRepository.HasOverlappingOperationOnMachineAsync(request.MachineId.Value, request.PlannedStartUtc, request.PlannedEndUtc, id, cancellationToken))
            throw new InvalidOperationException(SchedulingErrorMessages.OverlappingMachineOperationDetected);

        entity.WorkCenterId = request.WorkCenterId;
        entity.MachineId = request.MachineId;
        entity.PlannedShiftId = request.PlannedShiftId;
        entity.PlannedStartUtc = request.PlannedStartUtc;
        entity.PlannedEndUtc = request.PlannedEndUtc;
        entity.SetupTimeMinutes = request.SetupTimeMinutes;
        entity.RunTimeMinutes = request.RunTimeMinutes;
        entity.QueueTimeMinutes = request.QueueTimeMinutes;
        entity.WaitTimeMinutes = request.WaitTimeMinutes;
        entity.MoveTimeMinutes = request.MoveTimeMinutes;
        entity.PlannedQuantity = request.PlannedQuantity;
        entity.IsCriticalPath = request.IsCriticalPath;
        entity.IsBottleneckOperation = request.IsBottleneckOperation;
        entity.IsOutsourced = request.IsOutsourced;
        entity.PriorityScore = request.PriorityScore;
        entity.ConstraintReason = request.ConstraintReason?.Trim();
        entity.Notes = request.Notes?.Trim();
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _scheduleOperationRepository.UpdateAsync(entity, cancellationToken);
        return MapToResponse(entity);
    }

    public async Task<ScheduleOperationResponse> StartAsync(Guid id, StartScheduleOperationRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _scheduleOperationRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException(SchedulingErrorMessages.ScheduleOperationNotFound);

        entity.ActualShiftId = request.ActualShiftId;
        entity.ProductionExecutionId = request.ProductionExecutionId;
        entity.ActualStartUtc = request.ActualStartUtc;
        entity.Status = ScheduleOperationStatus.Running;
        entity.DispatchStatus = DispatchStatus.InExecution;
        entity.Notes = request.Notes?.Trim() ?? entity.Notes;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _scheduleOperationRepository.UpdateAsync(entity, cancellationToken);
        return MapToResponse(entity);
    }

    public async Task<ScheduleOperationResponse> PauseAsync(Guid id, PauseScheduleOperationRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _scheduleOperationRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException(SchedulingErrorMessages.ScheduleOperationNotFound);

        entity.Status = ScheduleOperationStatus.Paused;
        entity.Notes = request.Notes?.Trim() ?? entity.Notes;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _scheduleOperationRepository.UpdateAsync(entity, cancellationToken);
        return MapToResponse(entity);
    }

    public async Task<ScheduleOperationResponse> CompleteAsync(Guid id, CompleteScheduleOperationRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _scheduleOperationRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException(SchedulingErrorMessages.ScheduleOperationNotFound);

        entity.ActualEndUtc = request.ActualEndUtc;
        entity.CompletedQuantity = request.CompletedQuantity;
        entity.ScrappedQuantity = request.ScrappedQuantity;
        entity.ProductionExecutionId = request.ProductionExecutionId ?? entity.ProductionExecutionId;
        entity.Status = ScheduleOperationStatus.Completed;
        entity.DispatchStatus = DispatchStatus.Completed;
        entity.Notes = request.Notes?.Trim() ?? entity.Notes;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _scheduleOperationRepository.UpdateAsync(entity, cancellationToken);
        return MapToResponse(entity);
    }

    public async Task<ScheduleOperationResponse> RescheduleAsync(Guid id, RescheduleOperationRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _scheduleOperationRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException(SchedulingErrorMessages.ScheduleOperationNotFound);

        var workCenterId = request.NewWorkCenterId ?? entity.WorkCenterId;
        var machineId = request.NewMachineId ?? entity.MachineId;

        if (await _scheduleOperationRepository.HasOverlappingOperationOnWorkCenterAsync(workCenterId, request.NewPlannedStartUtc, request.NewPlannedEndUtc, id, cancellationToken))
            throw new InvalidOperationException(SchedulingErrorMessages.OverlappingWorkCenterOperationDetected);

        if (machineId.HasValue &&
            await _scheduleOperationRepository.HasOverlappingOperationOnMachineAsync(machineId.Value, request.NewPlannedStartUtc, request.NewPlannedEndUtc, id, cancellationToken))
            throw new InvalidOperationException(SchedulingErrorMessages.OverlappingMachineOperationDetected);

        entity.WorkCenterId = workCenterId;
        entity.MachineId = machineId;
        entity.PlannedShiftId = request.NewPlannedShiftId;
        entity.PlannedStartUtc = request.NewPlannedStartUtc;
        entity.PlannedEndUtc = request.NewPlannedEndUtc;
        if (request.NewPriorityScore.HasValue)
            entity.PriorityScore = request.NewPriorityScore.Value;
        entity.Status = ScheduleOperationStatus.Scheduled;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _scheduleOperationRepository.UpdateAsync(entity, cancellationToken);
        return MapToResponse(entity);
    }

    public async Task<ScheduleOperationDependencyResponse> AddDependencyAsync(CreateScheduleOperationDependencyRequest request, CancellationToken cancellationToken = default)
    {
        if (await _dependencyRepository.ExistsAsync(request.PredecessorOperationId, request.SuccessorOperationId, cancellationToken))
            throw new InvalidOperationException(SchedulingErrorMessages.DependencyAlreadyExists);

        var entity = new ScheduleOperationDependency
        {
            PredecessorOperationId = request.PredecessorOperationId,
            SuccessorOperationId = request.SuccessorOperationId,
            DependencyType = (DependencyType)request.DependencyType,
            LagMinutes = request.LagMinutes,
            IsMandatory = request.IsMandatory,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        await _dependencyRepository.AddAsync(entity, cancellationToken);
        var created = await _dependencyRepository.GetByIdAsync(entity.Id, cancellationToken) ?? entity;

        return new ScheduleOperationDependencyResponse
        {
            Id = created.Id,
            PredecessorOperationId = created.PredecessorOperationId,
            PredecessorOperationName = created.PredecessorOperation?.OperationName ?? string.Empty,
            SuccessorOperationId = created.SuccessorOperationId,
            SuccessorOperationName = created.SuccessorOperation?.OperationName ?? string.Empty,
            DependencyType = (int)created.DependencyType,
            DependencyTypeName = created.DependencyType.ToString(),
            LagMinutes = created.LagMinutes,
            IsMandatory = created.IsMandatory
        };
    }

    public async Task<bool> RemoveDependencyAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _dependencyRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return false;

        await _dependencyRepository.DeleteAsync(entity, cancellationToken);
        return true;
    }

    public async Task<ScheduleOperationConstraintResponse> AddConstraintAsync(CreateScheduleOperationConstraintRequest request, CancellationToken cancellationToken = default)
    {
        var entity = new ScheduleOperationConstraint
        {
            ScheduleOperationId = request.ScheduleOperationId,
            ConstraintType = (OperationConstraintType)request.ConstraintType,
            ReferenceNo = request.ReferenceNo.Trim(),
            Description = request.Description.Trim(),
            IsMandatory = request.IsMandatory,
            IsSatisfied = false,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        await _constraintRepository.AddAsync(entity, cancellationToken);
        return MapConstraint(entity);
    }

    public async Task<ScheduleOperationConstraintResponse> ResolveConstraintAsync(Guid id, ResolveScheduleOperationConstraintRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _constraintRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException(SchedulingErrorMessages.ConstraintNotFound);

        entity.IsSatisfied = request.IsSatisfied;
        entity.SatisfiedAtUtc = request.SatisfiedAtUtc;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _constraintRepository.UpdateAsync(entity, cancellationToken);
        return MapConstraint(entity);
    }

    public async Task<bool> RemoveConstraintAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _constraintRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return false;

        await _constraintRepository.DeleteAsync(entity, cancellationToken);
        return true;
    }

    public async Task<ScheduleOperationResourceOptionResponse> AddResourceOptionAsync(CreateScheduleOperationResourceOptionRequest request, CancellationToken cancellationToken = default)
    {
        var entity = new ScheduleOperationResourceOption
        {
            ScheduleOperationId = request.ScheduleOperationId,
            ResourceId = request.ResourceId,
            ResourceType = (ResourceType)request.ResourceType,
            IsPrimaryOption = request.IsPrimaryOption,
            PreferenceRank = request.PreferenceRank,
            EfficiencyFactor = request.EfficiencyFactor,
            SetupPenaltyMinutes = request.SetupPenaltyMinutes,
            IsActive = request.IsActive,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        await _resourceOptionRepository.AddAsync(entity, cancellationToken);

        return new ScheduleOperationResourceOptionResponse
        {
            Id = entity.Id,
            ScheduleOperationId = entity.ScheduleOperationId,
            ResourceId = entity.ResourceId,
            ResourceType = (int)entity.ResourceType,
            ResourceTypeName = entity.ResourceType.ToString(),
            IsPrimaryOption = entity.IsPrimaryOption,
            PreferenceRank = entity.PreferenceRank,
            EfficiencyFactor = entity.EfficiencyFactor,
            SetupPenaltyMinutes = entity.SetupPenaltyMinutes,
            IsActive = entity.IsActive
        };
    }

    public async Task<bool> RemoveResourceOptionAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _resourceOptionRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return false;

        await _resourceOptionRepository.DeleteAsync(entity, cancellationToken);
        return true;
    }

    public async Task<ScheduleResourceAssignmentResponse> AddResourceAssignmentAsync(CreateScheduleResourceAssignmentRequest request, CancellationToken cancellationToken = default)
    {
        if (await _resourceAssignmentRepository.HasOverlappingAssignmentAsync(request.ResourceId, (ResourceType)request.ResourceType, request.AssignedStartUtc, request.AssignedEndUtc, null, cancellationToken))
            throw new InvalidOperationException(SchedulingErrorMessages.OverlappingResourceAssignmentDetected);

        var entity = new ScheduleResourceAssignment
        {
            ScheduleOperationId = request.ScheduleOperationId,
            ResourceId = request.ResourceId,
            ResourceType = (ResourceType)request.ResourceType,
            ShiftId = request.ShiftId,
            AssignmentRole = request.AssignmentRole.Trim(),
            IsPrimary = request.IsPrimary,
            AssignedStartUtc = request.AssignedStartUtc,
            AssignedEndUtc = request.AssignedEndUtc,
            PlannedHours = request.PlannedHours,
            ActualHours = 0,
            Status = AssignmentStatus.Planned,
            Notes = request.Notes?.Trim(),
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        await _resourceAssignmentRepository.AddAsync(entity, cancellationToken);
        var created = await _resourceAssignmentRepository.GetByIdAsync(entity.Id, cancellationToken) ?? entity;
        return MapAssignment(created);
    }

    public async Task<ScheduleResourceAssignmentResponse> UpdateResourceAssignmentAsync(Guid id, UpdateScheduleResourceAssignmentRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _resourceAssignmentRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException(SchedulingErrorMessages.ResourceAssignmentNotFound);

        if (await _resourceAssignmentRepository.HasOverlappingAssignmentAsync(entity.ResourceId, entity.ResourceType, request.AssignedStartUtc, request.AssignedEndUtc, id, cancellationToken))
            throw new InvalidOperationException(SchedulingErrorMessages.OverlappingResourceAssignmentDetected);

        entity.ShiftId = request.ShiftId;
        entity.AssignmentRole = request.AssignmentRole.Trim();
        entity.IsPrimary = request.IsPrimary;
        entity.AssignedStartUtc = request.AssignedStartUtc;
        entity.AssignedEndUtc = request.AssignedEndUtc;
        entity.PlannedHours = request.PlannedHours;
        entity.ActualHours = request.ActualHours;
        entity.Status = (AssignmentStatus)request.Status;
        entity.Notes = request.Notes?.Trim();
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await _resourceAssignmentRepository.UpdateAsync(entity, cancellationToken);
        return MapAssignment(entity);
    }

    public async Task<bool> RemoveResourceAssignmentAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _resourceAssignmentRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return false;

        await _resourceAssignmentRepository.DeleteAsync(entity, cancellationToken);
        return true;
    }

    public async Task<ScheduleOperationDetailResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _scheduleOperationRepository.GetByIdWithDetailsAsync(id, cancellationToken);
        if (entity is null) return null;

        return new ScheduleOperationDetailResponse
        {
            Id = entity.Id,
            ScheduleJobId = entity.ScheduleJobId,
            JobNumber = entity.ScheduleJob?.JobNumber ?? string.Empty,
            RoutingStepId = entity.RoutingStepId,
            RoutingStepName = entity.RoutingStep?.OperationName ?? string.Empty,
            WorkCenterId = entity.WorkCenterId,
            WorkCenterName = entity.WorkCenter?.Name ?? string.Empty,
            MachineId = entity.MachineId,
            MachineName = entity.Machine?.Name,
            ProductionExecutionId = entity.ProductionExecutionId,
            PlannedShiftId = entity.PlannedShiftId,
            PlannedShiftName = entity.PlannedShift?.ShiftName,
            ActualShiftId = entity.ActualShiftId,
            ActualShiftName = entity.ActualShift?.ShiftName,
            SequenceNo = entity.SequenceNo,
            OperationCode = entity.OperationCode,
            OperationName = entity.OperationName,
            PlannedStartUtc = entity.PlannedStartUtc,
            PlannedEndUtc = entity.PlannedEndUtc,
            ActualStartUtc = entity.ActualStartUtc,
            ActualEndUtc = entity.ActualEndUtc,
            SetupTimeMinutes = entity.SetupTimeMinutes,
            RunTimeMinutes = entity.RunTimeMinutes,
            QueueTimeMinutes = entity.QueueTimeMinutes,
            WaitTimeMinutes = entity.WaitTimeMinutes,
            MoveTimeMinutes = entity.MoveTimeMinutes,
            PlannedQuantity = entity.PlannedQuantity,
            CompletedQuantity = entity.CompletedQuantity,
            ScrappedQuantity = entity.ScrappedQuantity,
            Status = (int)entity.Status,
            StatusName = entity.Status.ToString(),
            DispatchStatus = (int)entity.DispatchStatus,
            DispatchStatusName = entity.DispatchStatus.ToString(),
            IsCriticalPath = entity.IsCriticalPath,
            IsBottleneckOperation = entity.IsBottleneckOperation,
            IsOutsourced = entity.IsOutsourced,
            PriorityScore = entity.PriorityScore,
            ConstraintReason = entity.ConstraintReason,
            Notes = entity.Notes,
            CreatedAtUtc = entity.CreatedAtUtc,
            UpdatedAtUtc = entity.UpdatedAtUtc ?? entity.CreatedAtUtc,
            Dependencies = entity.PredecessorDependencies.Concat(entity.SuccessorDependencies)
                .Select(x => new ScheduleOperationDependencyResponse
                {
                    Id = x.Id,
                    PredecessorOperationId = x.PredecessorOperationId,
                    PredecessorOperationName = x.PredecessorOperation?.OperationName ?? string.Empty,
                    SuccessorOperationId = x.SuccessorOperationId,
                    SuccessorOperationName = x.SuccessorOperation?.OperationName ?? string.Empty,
                    DependencyType = (int)x.DependencyType,
                    DependencyTypeName = x.DependencyType.ToString(),
                    LagMinutes = x.LagMinutes,
                    IsMandatory = x.IsMandatory
                }).ToList(),
            Constraints = entity.Constraints.Select(MapConstraint).ToList(),
            ResourceOptions = entity.ResourceOptions.Select(x => new ScheduleOperationResourceOptionResponse
            {
                Id = x.Id,
                ScheduleOperationId = x.ScheduleOperationId,
                ResourceId = x.ResourceId,
                ResourceType = (int)x.ResourceType,
                ResourceTypeName = x.ResourceType.ToString(),
                IsPrimaryOption = x.IsPrimaryOption,
                PreferenceRank = x.PreferenceRank,
                EfficiencyFactor = x.EfficiencyFactor,
                SetupPenaltyMinutes = x.SetupPenaltyMinutes,
                IsActive = x.IsActive
            }).ToList(),
            ResourceAssignments = entity.ResourceAssignments.Select(MapAssignment).ToList(),
            CapacityReservations = entity.CapacityReservations.Select(x => new CapacityReservationResponse
            {
                Id = x.Id,
                ScheduleOperationId = x.ScheduleOperationId,
                ResourceId = x.ResourceId,
                ResourceType = (int)x.ResourceType,
                ResourceTypeName = x.ResourceType.ToString(),
                ShiftId = x.ShiftId,
                ShiftName = x.Shift?.ShiftName,
                ReservedStartUtc = x.ReservedStartUtc,
                ReservedEndUtc = x.ReservedEndUtc,
                ReservedMinutes = x.ReservedMinutes,
                AvailableMinutesAtBooking = x.AvailableMinutesAtBooking,
                Status = (int)x.Status,
                StatusName = x.Status.ToString(),
                ReservationReason = x.ReservationReason
            }).ToList(),
            DispatchQueueItems = entity.DispatchQueueItems.Select(x => new DispatchQueueItemResponse
            {
                Id = x.Id,
                ScheduleOperationId = x.ScheduleOperationId,
                WorkCenterId = x.WorkCenterId,
                WorkCenterName = x.WorkCenter?.Name ?? string.Empty,
                MachineId = x.MachineId,
                MachineName = x.Machine?.Name,
                QueuePosition = x.QueuePosition,
                PriorityScore = x.PriorityScore,
                Status = (int)x.Status,
                StatusName = x.Status.ToString(),
                ReleasedAtUtc = x.ReleasedAtUtc,
                AcknowledgedAtUtc = x.AcknowledgedAtUtc,
                DispatchNotes = x.DispatchNotes,
                IsActive = x.IsActive
            }).ToList()
        };
    }

    public async Task<PagedResponse<ScheduleOperationResponse>> GetAllAsync(GetScheduleOperationsRequest request, CancellationToken cancellationToken = default)
    {
        var (items, totalRecords) = await _scheduleOperationRepository.GetPagedAsync(request.PageNumber, request.PageSize, cancellationToken);

        return new PagedResponse<ScheduleOperationResponse>
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalRecords = totalRecords,
            Items = items.Select(MapToResponse).ToList()
        };
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _scheduleOperationRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null) return false;

        await _scheduleOperationRepository.DeleteAsync(entity, cancellationToken);
        return true;
    }

    private static ScheduleOperationResponse MapToResponse(ScheduleOperation entity)
    {
        return new ScheduleOperationResponse
        {
            Id = entity.Id,
            ScheduleJobId = entity.ScheduleJobId,
            JobNumber = entity.ScheduleJob?.JobNumber ?? string.Empty,
            RoutingStepId = entity.RoutingStepId,
            RoutingStepName = entity.RoutingStep?.OperationName ?? string.Empty,
            WorkCenterId = entity.WorkCenterId,
            WorkCenterName = entity.WorkCenter?.Name ?? string.Empty,
            MachineId = entity.MachineId,
            MachineName = entity.Machine?.Name,
            ProductionExecutionId = entity.ProductionExecutionId,
            PlannedShiftId = entity.PlannedShiftId,
            PlannedShiftName = entity.PlannedShift?.ShiftName,
            ActualShiftId = entity.ActualShiftId,
            ActualShiftName = entity.ActualShift?.ShiftName,
            SequenceNo = entity.SequenceNo,
            OperationCode = entity.OperationCode,
            OperationName = entity.OperationName,
            PlannedStartUtc = entity.PlannedStartUtc,
            PlannedEndUtc = entity.PlannedEndUtc,
            ActualStartUtc = entity.ActualStartUtc,
            ActualEndUtc = entity.ActualEndUtc,
            SetupTimeMinutes = entity.SetupTimeMinutes,
            RunTimeMinutes = entity.RunTimeMinutes,
            QueueTimeMinutes = entity.QueueTimeMinutes,
            WaitTimeMinutes = entity.WaitTimeMinutes,
            MoveTimeMinutes = entity.MoveTimeMinutes,
            PlannedQuantity = entity.PlannedQuantity,
            CompletedQuantity = entity.CompletedQuantity,
            ScrappedQuantity = entity.ScrappedQuantity,
            Status = (int)entity.Status,
            StatusName = entity.Status.ToString(),
            DispatchStatus = (int)entity.DispatchStatus,
            DispatchStatusName = entity.DispatchStatus.ToString(),
            IsCriticalPath = entity.IsCriticalPath,
            IsBottleneckOperation = entity.IsBottleneckOperation,
            IsOutsourced = entity.IsOutsourced,
            PriorityScore = entity.PriorityScore,
            ConstraintReason = entity.ConstraintReason,
            Notes = entity.Notes,
            CreatedAtUtc = entity.CreatedAtUtc,
            UpdatedAtUtc = entity.UpdatedAtUtc ?? entity.CreatedAtUtc
        };
    }

    private static ScheduleOperationConstraintResponse MapConstraint(ScheduleOperationConstraint entity)
    {
        return new ScheduleOperationConstraintResponse
        {
            Id = entity.Id,
            ScheduleOperationId = entity.ScheduleOperationId,
            ConstraintType = (int)entity.ConstraintType,
            ConstraintTypeName = entity.ConstraintType.ToString(),
            ReferenceNo = entity.ReferenceNo,
            Description = entity.Description,
            IsSatisfied = entity.IsSatisfied,
            SatisfiedAtUtc = entity.SatisfiedAtUtc,
            IsMandatory = entity.IsMandatory
        };
    }

    private static ScheduleResourceAssignmentResponse MapAssignment(ScheduleResourceAssignment entity)
    {
        return new ScheduleResourceAssignmentResponse
        {
            Id = entity.Id,
            ScheduleOperationId = entity.ScheduleOperationId,
            ResourceId = entity.ResourceId,
            ResourceType = (int)entity.ResourceType,
            ResourceTypeName = entity.ResourceType.ToString(),
            ShiftId = entity.ShiftId,
            ShiftName = entity.Shift?.ShiftName,
            AssignmentRole = entity.AssignmentRole,
            IsPrimary = entity.IsPrimary,
            AssignedStartUtc = entity.AssignedStartUtc,
            AssignedEndUtc = entity.AssignedEndUtc,
            PlannedHours = entity.PlannedHours,
            ActualHours = entity.ActualHours,
            Status = (int)entity.Status,
            StatusName = entity.Status.ToString(),
            Notes = entity.Notes
        };
    }
}
