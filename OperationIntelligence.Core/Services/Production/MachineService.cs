using Microsoft.EntityFrameworkCore;
using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class MachineService : IMachineService
{
    private readonly IMachineRepository _machineRepository;
    private readonly IWorkCenterRepository _workCenterRepository;

    public MachineService(IMachineRepository machineRepository, IWorkCenterRepository workCenterRepository)
    {
        _machineRepository = machineRepository;
        _workCenterRepository = workCenterRepository;
    }

    public async Task<MachineResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _machineRepository.GetWithWorkCenterAsync(id, cancellationToken);
        return entity is null || entity.IsDeleted ? null : entity.ToResponse();
    }

    public async Task<PagedResponse<MachineResponse>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        pageNumber = pageNumber <= 0 ? 1 : pageNumber;
        pageSize = pageSize <= 0 ? 10 : pageSize;

        var query = _machineRepository.Query().AsNoTracking().Where(x => !x.IsDeleted).OrderBy(x => x.Name);
        var total = await query.CountAsync(cancellationToken);
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).Select(x => new MachineResponse
        {
            Id = x.Id,
            MachineCode = x.MachineCode,
            Name = x.Name,
            WorkCenterId = x.WorkCenterId,
            WorkCenterName = x.WorkCenter.Name,
            Model = x.Model,
            Manufacturer = x.Manufacturer,
            SerialNumber = x.SerialNumber,
            HourlyRunningCost = x.HourlyRunningCost,
            Status = x.Status,
            LastMaintenanceDate = x.LastMaintenanceDate,
            NextMaintenanceDate = x.NextMaintenanceDate,
            IsActive = x.IsActive,
            CreatedAtUtc = x.CreatedAtUtc,
            CreatedBy = x.CreatedBy,
            UpdatedAtUtc = x.UpdatedAtUtc,
            UpdatedBy = x.UpdatedBy
        }).ToListAsync(cancellationToken);

        return new PagedResponse<MachineResponse> { PageNumber = pageNumber, PageSize = pageSize, TotalRecords = total, Items = items };
    }

    public async Task<MachineResponse> CreateAsync(CreateMachineRequest request, string? createdBy = null, CancellationToken cancellationToken = default)
    {
        var workCenterExists = await _workCenterRepository.ExistsAsync(x => x.Id == request.WorkCenterId && !x.IsDeleted && x.IsActive, cancellationToken);
        if (!workCenterExists) throw new InvalidOperationException(ProductionErrorMessages.WorkCenterDoesNotExistOrIsInactive);

        var codeExists = await _machineRepository.MachineCodeExistsAsync(request.MachineCode.Trim(), null, cancellationToken);
        if (codeExists) throw new InvalidOperationException(ProductionErrorMessages.MachineCodeAlreadyExists);

        var entity = new Machine
        {
            MachineCode = request.MachineCode.Trim(),
            Name = request.Name.Trim(),
            WorkCenterId = request.WorkCenterId,
            Model = request.Model?.Trim(),
            Manufacturer = request.Manufacturer?.Trim(),
            SerialNumber = request.SerialNumber?.Trim(),
            HourlyRunningCost = request.HourlyRunningCost,
            Status = request.Status,
            LastMaintenanceDate = request.LastMaintenanceDate,
            NextMaintenanceDate = request.NextMaintenanceDate,
            IsActive = request.IsActive,
            CreatedBy = createdBy
        };

        await _machineRepository.AddAsync(entity, cancellationToken);
        await _machineRepository.SaveChangesAsync(cancellationToken);
        return entity.ToResponse();
    }

    public async Task<MachineResponse?> UpdateAsync(Guid id, UpdateMachineRequest request, string? updatedBy = null, CancellationToken cancellationToken = default)
    {
        var entity = await _machineRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null || entity.IsDeleted) return null;

        var workCenterExists = await _workCenterRepository.ExistsAsync(x => x.Id == request.WorkCenterId && !x.IsDeleted && x.IsActive, cancellationToken);
        if (!workCenterExists) throw new InvalidOperationException(ProductionErrorMessages.WorkCenterDoesNotExistOrIsInactive);

        var codeExists = await _machineRepository.MachineCodeExistsAsync(request.MachineCode.Trim(), id, cancellationToken);
        if (codeExists) throw new InvalidOperationException(ProductionErrorMessages.MachineCodeAlreadyExists);

        entity.MachineCode = request.MachineCode.Trim();
        entity.Name = request.Name.Trim();
        entity.WorkCenterId = request.WorkCenterId;
        entity.Model = request.Model?.Trim();
        entity.Manufacturer = request.Manufacturer?.Trim();
        entity.SerialNumber = request.SerialNumber?.Trim();
        entity.HourlyRunningCost = request.HourlyRunningCost;
        entity.Status = request.Status;
        entity.LastMaintenanceDate = request.LastMaintenanceDate;
        entity.NextMaintenanceDate = request.NextMaintenanceDate;
        entity.IsActive = request.IsActive;
        entity.UpdatedAtUtc = DateTime.UtcNow;
        entity.UpdatedBy = updatedBy;

        _machineRepository.Update(entity);
        await _machineRepository.SaveChangesAsync(cancellationToken);
        return entity.ToResponse();
    }

    public async Task<bool> DeleteAsync(Guid id, string? deletedBy = null, CancellationToken cancellationToken = default)
    {
        var entity = await _machineRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null || entity.IsDeleted) return false;

        entity.IsDeleted = true;
        entity.IsActive = false;
        entity.DeletedAtUtc = DateTime.UtcNow;
        entity.DeletedBy = deletedBy;

        _machineRepository.Update(entity);
        await _machineRepository.SaveChangesAsync(cancellationToken);
        return true;
    }
}
