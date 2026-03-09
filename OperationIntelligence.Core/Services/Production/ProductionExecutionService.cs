using Microsoft.EntityFrameworkCore;
using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class ProductionExecutionService : IProductionExecutionService
{
    private readonly IProductionExecutionRepository _executionRepository;
    private readonly IProductionOrderRepository _orderRepository;
    private readonly IWorkCenterRepository _workCenterRepository;

    public ProductionExecutionService(
        IProductionExecutionRepository executionRepository,
        IProductionOrderRepository orderRepository,
        IWorkCenterRepository workCenterRepository)
    {
        _executionRepository = executionRepository;
        _orderRepository = orderRepository;
        _workCenterRepository = workCenterRepository;
    }

    public async Task<ProductionExecutionResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _executionRepository.GetWithDetailsAsync(id, cancellationToken);
        return entity is null || entity.IsDeleted ? null : entity.ToResponse();
    }

    public async Task<PagedResponse<ProductionExecutionSummaryResponse>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        pageNumber = pageNumber <= 0 ? 1 : pageNumber;
        pageSize = pageSize <= 0 ? 10 : pageSize;

        var query = _executionRepository.Query().AsNoTracking().Where(x => !x.IsDeleted).OrderByDescending(x => x.CreatedAtUtc);
        var total = await query.CountAsync(cancellationToken);
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).Select(x => new ProductionExecutionSummaryResponse
        {
            Id = x.Id,
            ProductionOrderId = x.ProductionOrderId,
            ProductionOrderNumber = x.ProductionOrder.ProductionOrderNumber,
            WorkCenterId = x.WorkCenterId,
            WorkCenterName = x.WorkCenter.Name,
            MachineId = x.MachineId,
            MachineName = x.Machine != null ? x.Machine.Name : null,
            PlannedQuantity = x.PlannedQuantity,
            CompletedQuantity = x.CompletedQuantity,
            ScrapQuantity = x.ScrapQuantity,
            PlannedStartDate = x.PlannedStartDate,
            PlannedEndDate = x.PlannedEndDate,
            Status = x.Status
        }).ToListAsync(cancellationToken);

        return new PagedResponse<ProductionExecutionSummaryResponse> { PageNumber = pageNumber, PageSize = pageSize, TotalRecords = total, Items = items };
    }

    public async Task<ProductionExecutionResponse> CreateAsync(CreateProductionExecutionRequest request, string? createdBy = null, CancellationToken cancellationToken = default)
    {
        var orderExists = await _orderRepository.ExistsAsync(x => x.Id == request.ProductionOrderId && !x.IsDeleted, cancellationToken);
        if (!orderExists) throw new InvalidOperationException("Production order does not exist.");

        var workCenterExists = await _workCenterRepository.ExistsAsync(x => x.Id == request.WorkCenterId && !x.IsDeleted && x.IsActive, cancellationToken);
        if (!workCenterExists) throw new InvalidOperationException("Work center does not exist or is inactive.");

        var entity = new ProductionExecution
        {
            ProductionOrderId = request.ProductionOrderId,
            RoutingStepId = request.RoutingStepId,
            WorkCenterId = request.WorkCenterId,
            MachineId = request.MachineId,
            PlannedQuantity = request.PlannedQuantity,
            PlannedStartDate = request.PlannedStartDate,
            PlannedEndDate = request.PlannedEndDate,
            Status = ExecutionStatus.Pending,
            Remarks = request.Remarks?.Trim(),
            CreatedBy = createdBy
        };

        await _executionRepository.AddAsync(entity, cancellationToken);
        await _executionRepository.SaveChangesAsync(cancellationToken);

        var created = await _executionRepository.GetWithDetailsAsync(entity.Id, cancellationToken) ?? entity;
        return created.ToResponse();
    }
}
