using Microsoft.EntityFrameworkCore;
using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class WorkCenterService : IWorkCenterService
{
    private readonly IWorkCenterRepository _workCenterRepository;
    private readonly IWarehouseRepository _warehouseRepository;

    public WorkCenterService(IWorkCenterRepository workCenterRepository, IWarehouseRepository warehouseRepository)
    {
        _workCenterRepository = workCenterRepository;
        _warehouseRepository = warehouseRepository;
    }

    public async Task<WorkCenterResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _workCenterRepository.GetWithMachinesAsync(id, cancellationToken);
        return entity is null || entity.IsDeleted ? null : entity.ToResponse();
    }

    public async Task<PagedResponse<WorkCenterResponse>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        pageNumber = pageNumber <= 0 ? 1 : pageNumber;
        pageSize = pageSize <= 0 ? 10 : pageSize;

        var query = _workCenterRepository.Query().AsNoTracking().Where(x => !x.IsDeleted).OrderBy(x => x.Name);
        var total = await query.CountAsync(cancellationToken);
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).Select(x => new WorkCenterResponse
        {
            Id = x.Id,
            Code = x.Code,
            Name = x.Name,
            Description = x.Description,
            WarehouseId = x.WarehouseId,
            WarehouseName = x.Warehouse.Name,
            CapacityPerDay = x.CapacityPerDay,
            AvailableOperators = x.AvailableOperators,
            IsActive = x.IsActive,
            CreatedAtUtc = x.CreatedAtUtc,
            CreatedBy = x.CreatedBy,
            UpdatedAtUtc = x.UpdatedAtUtc,
            UpdatedBy = x.UpdatedBy
        }).ToListAsync(cancellationToken);

        return new PagedResponse<WorkCenterResponse> { PageNumber = pageNumber, PageSize = pageSize, TotalRecords = total, Items = items };
    }

    public async Task<WorkCenterResponse> CreateAsync(CreateWorkCenterRequest request, string? createdBy = null, CancellationToken cancellationToken = default)
    {
        var warehouseExists = await _warehouseRepository.ExistsAsync(x => x.Id == request.WarehouseId && !x.IsDeleted, cancellationToken);
        if (!warehouseExists) throw new InvalidOperationException("Warehouse does not exist.");

        var codeExists = await _workCenterRepository.CodeExistsAsync(request.Code.Trim(), null, cancellationToken);
        if (codeExists) throw new InvalidOperationException("Work center code already exists.");

        var entity = new WorkCenter
        {
            Code = request.Code.Trim(),
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            WarehouseId = request.WarehouseId,
            CapacityPerDay = request.CapacityPerDay,
            AvailableOperators = request.AvailableOperators,
            IsActive = request.IsActive,
            CreatedBy = createdBy
        };

        await _workCenterRepository.AddAsync(entity, cancellationToken);
        await _workCenterRepository.SaveChangesAsync(cancellationToken);
        return entity.ToResponse();
    }

    public async Task<WorkCenterResponse?> UpdateAsync(Guid id, UpdateWorkCenterRequest request, string? updatedBy = null, CancellationToken cancellationToken = default)
    {
        var entity = await _workCenterRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null || entity.IsDeleted) return null;

        var warehouseExists = await _warehouseRepository.ExistsAsync(x => x.Id == request.WarehouseId && !x.IsDeleted, cancellationToken);
        if (!warehouseExists) throw new InvalidOperationException("Warehouse does not exist.");

        var codeExists = await _workCenterRepository.CodeExistsAsync(request.Code.Trim(), id, cancellationToken);
        if (codeExists) throw new InvalidOperationException("Work center code already exists.");

        entity.Code = request.Code.Trim();
        entity.Name = request.Name.Trim();
        entity.Description = request.Description?.Trim();
        entity.WarehouseId = request.WarehouseId;
        entity.CapacityPerDay = request.CapacityPerDay;
        entity.AvailableOperators = request.AvailableOperators;
        entity.IsActive = request.IsActive;
        entity.UpdatedAtUtc = DateTime.UtcNow;
        entity.UpdatedBy = updatedBy;

        _workCenterRepository.Update(entity);
        await _workCenterRepository.SaveChangesAsync(cancellationToken);
        return entity.ToResponse();
    }

    public async Task<bool> DeleteAsync(Guid id, string? deletedBy = null, CancellationToken cancellationToken = default)
    {
        var entity = await _workCenterRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null || entity.IsDeleted) return false;

        entity.IsDeleted = true;
        entity.IsActive = false;
        entity.DeletedAtUtc = DateTime.UtcNow;
        entity.DeletedBy = deletedBy;

        _workCenterRepository.Update(entity);
        await _workCenterRepository.SaveChangesAsync(cancellationToken);
        return true;
    }
}
