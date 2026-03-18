using Microsoft.EntityFrameworkCore;

namespace OperationIntelligence.DB;

public class DashboardReadRepository : IDashboardReadRepository
{
    private readonly OperationIntelligenceDbContext _db;

    private sealed class InventoryDashboardRow
    {
        public InventoryStock Stock { get; init; } = default!;
        public Product Product { get; init; } = default!;
    }

    public DashboardReadRepository(OperationIntelligenceDbContext db)
    {
        _db = db;
    }

    public async Task<DashboardOverviewReadModel> GetOverviewAsync(
        string range,
        string site,
        CancellationToken cancellationToken = default)
    {
        var fromDate = ResolveFromDate(range);
        var warehouseIds = await ResolveWarehouseIdsAsync(site, cancellationToken);

        return new DashboardOverviewReadModel
        {
            Kpis = await GetKpisAsync(fromDate, warehouseIds, cancellationToken),
            Alerts = await GetAlertsAsync(fromDate, warehouseIds, cancellationToken),
            ModuleHealth = await GetModuleHealthAsync(fromDate, warehouseIds, cancellationToken),
            BusinessPerformance = await GetBusinessPerformanceAsync(fromDate, warehouseIds, cancellationToken),
            Finance = await GetFinanceAnalyticsAsync(fromDate, warehouseIds, cancellationToken),
            Inventory = await GetInventoryAnalyticsAsync(fromDate, warehouseIds, cancellationToken),
            Production = await GetProductionAnalyticsAsync(fromDate, warehouseIds, cancellationToken),
            Shipment = await GetShipmentAnalyticsAsync(fromDate, warehouseIds, cancellationToken),
            WorkflowPipeline = await GetWorkflowPipelineAsync(fromDate, warehouseIds, cancellationToken),
            ActivityFeed = await GetActivityFeedAsync(fromDate, warehouseIds, cancellationToken),
            WorkforceSummary = await GetWorkforceSummaryAsync(cancellationToken),
            ProcurementSummary = await GetProcurementSummaryAsync(fromDate, warehouseIds, cancellationToken),
            WarehouseSummary = await GetWarehouseSummaryAsync(fromDate, warehouseIds, cancellationToken),
            RecentOrders = await GetRecentOrdersAsync(fromDate, warehouseIds, cancellationToken),
            LowStockItems = await GetLowStockItemsAsync(warehouseIds, cancellationToken)
        };
    }

    private static DateTime ResolveFromDate(string? range)
    {
        var now = DateTime.UtcNow;

        return range?.Trim().ToLowerInvariant() switch
        {
            "7d" => now.AddDays(-7),
            "30d" => now.AddDays(-30),
            "90d" => now.AddDays(-90),
            "1y" => now.AddYears(-1),
            _ => now.AddDays(-30)
        };
    }

    private static bool HasWarehouseFilter(IReadOnlyCollection<Guid> warehouseIds)
        => warehouseIds.Count > 0;

    private async Task<List<Guid>> ResolveWarehouseIdsAsync(string? site, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(site) || site.Equals("all", StringComparison.OrdinalIgnoreCase))
        {
            return [];
        }

        var normalized = site.Trim().ToLowerInvariant();

        return await _db.Warehouses
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .Where(x =>
                x.Name.ToLower().Contains(normalized) ||
                x.Code.ToLower().Contains(normalized) ||
                (x.City != null && x.City.ToLower().Contains(normalized)))
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    private async Task<DashboardKpisReadModel> GetKpisAsync(
        DateTime fromDate,
        List<Guid> warehouseIds,
        CancellationToken cancellationToken)
    {
        var hasWarehouseFilter = HasWarehouseFilter(warehouseIds);

        var paymentsQuery = _db.OrderPayments
            .AsNoTracking()
            .Where(x => x.PaymentDateUtc >= fromDate && x.Status == PaymentStatus.Paid && x.IsActive)
            .Join(_db.Orders.AsNoTracking(), payment => payment.OrderId, order => order.Id, (payment, order) => new { payment, order });

        if (hasWarehouseFilter)
        {
            paymentsQuery = paymentsQuery.Where(x => x.order.WarehouseId.HasValue && warehouseIds.Contains(x.order.WarehouseId.Value));
        }

        var totalRevenue = await paymentsQuery.SumAsync(x => (decimal?)x.payment.Amount, cancellationToken) ?? 0m;

        var ordersQuery = _db.Orders.AsNoTracking().Where(x => x.OrderDateUtc >= fromDate);
        if (hasWarehouseFilter)
        {
            ordersQuery = ordersQuery.Where(x => x.WarehouseId.HasValue && warehouseIds.Contains(x.WarehouseId.Value));
        }

        var ordersInProgress = await ordersQuery.CountAsync(
            x => x.Status == OrderStatus.PendingApproval ||
                 x.Status == OrderStatus.Approved ||
                 x.Status == OrderStatus.Processing ||
                 x.Status == OrderStatus.PartiallyFulfilled,
            cancellationToken);

        var inventoryValueQuery = GetInventoryBaseQuery(warehouseIds);
        var inventoryValue = await inventoryValueQuery
            .SumAsync(x => (decimal?)x.Stock.QuantityOnHand * x.Product.CostPrice, cancellationToken) ?? 0m;

        var shipmentsQuery = _db.Shipments.AsNoTracking().Where(x => !x.IsDeleted && x.CreatedAtUtc >= fromDate);
        if (hasWarehouseFilter)
        {
            shipmentsQuery = shipmentsQuery.Where(x => warehouseIds.Contains(x.WarehouseId));
        }

        var shipmentsPending = await shipmentsQuery.CountAsync(
            x => x.Status == ShipmentStatus.AwaitingAllocation ||
                 x.Status == ShipmentStatus.Allocated ||
                 x.Status == ShipmentStatus.Picking ||
                 x.Status == ShipmentStatus.Packed ||
                 x.Status == ShipmentStatus.ReadyToDispatch ||
                 x.Status == ShipmentStatus.InTransit,
            cancellationToken);

        var lowStockCount = await GetLowStockBaseQuery(warehouseIds).CountAsync(cancellationToken);

        var exceptionQuery = _db.ShipmentExceptions
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .Join(_db.Shipments.AsNoTracking().Where(x => !x.IsDeleted), ex => ex.ShipmentId, shipment => shipment.Id, (ex, shipment) => new { ex, shipment })
            .Where(x => x.ex.ReportedAtUtc >= fromDate && !x.ex.IsResolved);

        if (hasWarehouseFilter)
        {
            exceptionQuery = exceptionQuery.Where(x => warehouseIds.Contains(x.shipment.WarehouseId));
        }

        var unresolvedExceptions = await exceptionQuery.CountAsync(cancellationToken);

        var executionQuery = _db.ProductionExecutions
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .Join(_db.WorkCenters.AsNoTracking().Where(x => !x.IsDeleted), execution => execution.WorkCenterId, workCenter => workCenter.Id, (execution, workCenter) => new { execution, workCenter })
            .Where(x => x.execution.CreatedAtUtc >= fromDate);

        if (hasWarehouseFilter)
        {
            executionQuery = executionQuery.Where(x => warehouseIds.Contains(x.workCenter.WarehouseId));
        }

        var executionRatios = await executionQuery
            .Select(x => x.execution.PlannedQuantity > 0m
                ? (x.execution.CompletedQuantity / x.execution.PlannedQuantity) * 100m
                : 0m)
            .ToListAsync(cancellationToken);

        var productionEfficiency = executionRatios.Count == 0 ? 0m : executionRatios.Average();

        return new DashboardKpisReadModel
        {
            TotalRevenue = Math.Round(totalRevenue, 2),
            OrdersInProgress = ordersInProgress,
            ProductionEfficiency = Math.Round(productionEfficiency, 2),
            InventoryValue = Math.Round(inventoryValue, 2),
            ShipmentsPending = shipmentsPending,
            CriticalAlerts = lowStockCount + unresolvedExceptions
        };
    }

    private async Task<List<DashboardAlertReadModel>> GetAlertsAsync(
        DateTime fromDate,
        List<Guid> warehouseIds,
        CancellationToken cancellationToken)
    {
        var lowStockCount = await GetLowStockBaseQuery(warehouseIds).CountAsync(cancellationToken);

        var delayedShipmentQuery = _db.ShipmentExceptions
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .Join(_db.Shipments.AsNoTracking().Where(x => !x.IsDeleted), ex => ex.ShipmentId, shipment => shipment.Id, (ex, shipment) => new { ex, shipment })
            .Where(x => x.ex.ReportedAtUtc >= fromDate && x.ex.ExceptionType == ShipmentExceptionType.Delay);

        if (HasWarehouseFilter(warehouseIds))
        {
            delayedShipmentQuery = delayedShipmentQuery.Where(x => warehouseIds.Contains(x.shipment.WarehouseId));
        }

        var delayedShipments = await delayedShipmentQuery.CountAsync(cancellationToken);

        var pendingOrdersQuery = _db.Orders.AsNoTracking().Where(x => x.OrderDateUtc >= fromDate && x.Status == OrderStatus.PendingApproval);
        if (HasWarehouseFilter(warehouseIds))
        {
            pendingOrdersQuery = pendingOrdersQuery.Where(x => x.WarehouseId.HasValue && warehouseIds.Contains(x.WarehouseId.Value));
        }

        var pendingApprovals = await pendingOrdersQuery.CountAsync(cancellationToken);

        var underTargetQuery = _db.ProductionExecutions
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .Join(_db.WorkCenters.AsNoTracking().Where(x => !x.IsDeleted), execution => execution.WorkCenterId, workCenter => workCenter.Id, (execution, workCenter) => new { execution, workCenter })
            .Where(x => x.execution.CreatedAtUtc >= fromDate)
            .Where(x => x.execution.PlannedQuantity > 0m && x.execution.CompletedQuantity < x.execution.PlannedQuantity);

        if (HasWarehouseFilter(warehouseIds))
        {
            underTargetQuery = underTargetQuery.Where(x => warehouseIds.Contains(x.workCenter.WarehouseId));
        }

        var linesUnderTarget = await underTargetQuery.CountAsync(cancellationToken);

        return
        [
            new DashboardAlertReadModel
            {
                Title = $"{lowStockCount} items below reorder level",
                Detail = "Inventory replenishment required across warehouses.",
                Severity = lowStockCount >= 10 ? "Critical" : "Warning"
            },
            new DashboardAlertReadModel
            {
                Title = $"{delayedShipments} delayed shipment exceptions",
                Detail = "Shipment exceptions need review.",
                Severity = delayedShipments >= 5 ? "Warning" : "Info"
            },
            new DashboardAlertReadModel
            {
                Title = $"{pendingApprovals} orders awaiting approval",
                Detail = "Commercial approval queue is building.",
                Severity = pendingApprovals >= 10 ? "Warning" : "Info"
            },
            new DashboardAlertReadModel
            {
                Title = $"{linesUnderTarget} executions below plan",
                Detail = "Production throughput is below expected output.",
                Severity = linesUnderTarget >= 5 ? "Warning" : "Info"
            }
        ];
    }

    private async Task<List<DashboardModuleHealthReadModel>> GetModuleHealthAsync(
        DateTime fromDate,
        List<Guid> warehouseIds,
        CancellationToken cancellationToken)
    {
        var lowStockCount = await GetLowStockBaseQuery(warehouseIds).CountAsync(cancellationToken);

        var ordersQuery = _db.Orders.AsNoTracking().Where(x => x.OrderDateUtc >= fromDate);
        if (HasWarehouseFilter(warehouseIds))
        {
            ordersQuery = ordersQuery.Where(x => x.WarehouseId.HasValue && warehouseIds.Contains(x.WarehouseId.Value));
        }

        var pendingOrders = await ordersQuery.CountAsync(
            x => x.Status == OrderStatus.PendingApproval || x.Status == OrderStatus.Processing,
            cancellationToken);

        var qualityQuery = _db.ProductionQualityChecks
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .Join(_db.ProductionOrders.AsNoTracking().Where(x => !x.IsDeleted), qc => qc.ProductionOrderId, po => po.Id, (qc, po) => new { qc, po })
            .Where(x => x.qc.CheckDate >= fromDate && x.qc.Status == QualityCheckStatus.Failed);

        if (HasWarehouseFilter(warehouseIds))
        {
            qualityQuery = qualityQuery.Where(x => warehouseIds.Contains(x.po.WarehouseId));
        }

        var failedQualityChecks = await qualityQuery.CountAsync(cancellationToken);

        var shipmentIssuesQuery = _db.ShipmentExceptions
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .Join(_db.Shipments.AsNoTracking().Where(x => !x.IsDeleted), ex => ex.ShipmentId, shipment => shipment.Id, (ex, shipment) => new { ex, shipment })
            .Where(x => x.ex.ReportedAtUtc >= fromDate && !x.ex.IsResolved);

        if (HasWarehouseFilter(warehouseIds))
        {
            shipmentIssuesQuery = shipmentIssuesQuery.Where(x => warehouseIds.Contains(x.shipment.WarehouseId));
        }

        var shipmentIssues = await shipmentIssuesQuery.CountAsync(cancellationToken);

        return
        [
            new DashboardModuleHealthReadModel
            {
                Module = "Inventory",
                Status = lowStockCount > 0 ? "Warning" : "Healthy",
                Value = lowStockCount.ToString(),
                Note = "Items below reorder threshold"
            },
            new DashboardModuleHealthReadModel
            {
                Module = "Orders",
                Status = pendingOrders > 15 ? "Warning" : "Healthy",
                Value = pendingOrders.ToString(),
                Note = "Orders awaiting approval or fulfillment"
            },
            new DashboardModuleHealthReadModel
            {
                Module = "Production",
                Status = failedQualityChecks > 0 ? "Warning" : "Healthy",
                Value = failedQualityChecks.ToString(),
                Note = "Failed quality checks in selected range"
            },
            new DashboardModuleHealthReadModel
            {
                Module = "Shipment",
                Status = shipmentIssues > 0 ? "Warning" : "Healthy",
                Value = shipmentIssues.ToString(),
                Note = "Open shipment exceptions"
            }
        ];
    }

    private async Task<BusinessPerformanceReadModel> GetBusinessPerformanceAsync(
        DateTime fromDate,
        List<Guid> warehouseIds,
        CancellationToken cancellationToken)
    {
        var paymentQuery = _db.OrderPayments
            .AsNoTracking()
            .Where(x => x.PaymentDateUtc >= fromDate && x.Status == PaymentStatus.Paid && x.IsActive)
            .Join(_db.Orders.AsNoTracking(), payment => payment.OrderId, order => order.Id, (payment, order) => new { payment, order });

        if (HasWarehouseFilter(warehouseIds))
        {
            paymentQuery = paymentQuery.Where(x => x.order.WarehouseId.HasValue && warehouseIds.Contains(x.order.WarehouseId.Value));
        }

        var monthlyRevenue = await paymentQuery
            .Select(x => new { x.payment.PaymentDateUtc, x.payment.Amount })
            .ToListAsync(cancellationToken);

        var shipmentQuery = _db.Shipments.AsNoTracking().Where(x => !x.IsDeleted && x.CreatedAtUtc >= fromDate);
        if (HasWarehouseFilter(warehouseIds))
        {
            shipmentQuery = shipmentQuery.Where(x => warehouseIds.Contains(x.WarehouseId));
        }

        var deliveredShipments = await shipmentQuery
            .Where(x => x.ActualDeliveryDateUtc.HasValue && x.PlannedDeliveryDateUtc.HasValue)
            .Select(x => new { x.ActualDeliveryDateUtc, x.PlannedDeliveryDateUtc })
            .ToListAsync(cancellationToken);

        var inventoryQuery = _db.InventoryStocks.AsNoTracking().Where(x => !x.IsDeleted);
        if (HasWarehouseFilter(warehouseIds))
        {
            inventoryQuery = inventoryQuery.Where(x => warehouseIds.Contains(x.WarehouseId));
        }

        var inventorySnapshot = await inventoryQuery
            .Select(x => new { x.QuantityOnHand, x.QuantityReserved })
            .ToListAsync(cancellationToken);

        var approvalQueueQuery = _db.Orders.AsNoTracking().Where(x => x.OrderDateUtc >= fromDate && x.Status == OrderStatus.PendingApproval);
        if (HasWarehouseFilter(warehouseIds))
        {
            approvalQueueQuery = approvalQueueQuery.Where(x => x.WarehouseId.HasValue && warehouseIds.Contains(x.WarehouseId.Value));
        }

        var approvalQueue = await approvalQueueQuery.CountAsync(cancellationToken);

        var monthlyRevenueTrend = monthlyRevenue
            .GroupBy(x => new { x.PaymentDateUtc.Year, x.PaymentDateUtc.Month })
            .OrderBy(x => x.Key.Year).ThenBy(x => x.Key.Month)
            .Select(x => new ChartPointReadModel
            {
                Label = $"{x.Key.Year}-{x.Key.Month:00}",
                Value = x.Sum(y => y.Amount)
            })
            .ToList();

        var onTimeShipments = deliveredShipments.Count(x => x.ActualDeliveryDateUtc <= x.PlannedDeliveryDateUtc);
        var onTimeRate = deliveredShipments.Count == 0 ? 0m : (decimal)onTimeShipments / deliveredShipments.Count * 100m;

        var totalOnHand = inventorySnapshot.Sum(x => x.QuantityOnHand);
        var totalReserved = inventorySnapshot.Sum(x => x.QuantityReserved);
        var capacityUse = totalOnHand <= 0m ? 0m : totalReserved / totalOnHand * 100m;

        return new BusinessPerformanceReadModel
        {
            MonthlyRevenueTrend = monthlyRevenueTrend,
            OnTimeShipmentRate = Math.Round(onTimeRate, 2),
            WarehouseCapacityUse = Math.Round(capacityUse, 2),
            ApprovalQueue = approvalQueue
        };
    }

    private async Task<FinanceAnalyticsReadModel> GetFinanceAnalyticsAsync(
        DateTime fromDate,
        List<Guid> warehouseIds,
        CancellationToken cancellationToken)
    {
        var paymentsQuery = _db.OrderPayments
            .AsNoTracking()
            .Where(x => x.PaymentDateUtc >= fromDate && x.Status == PaymentStatus.Paid && x.IsActive)
            .Join(_db.Orders.AsNoTracking(), payment => payment.OrderId, order => order.Id, (payment, order) => new { payment, order });

        if (HasWarehouseFilter(warehouseIds))
        {
            paymentsQuery = paymentsQuery.Where(x => x.order.WarehouseId.HasValue && warehouseIds.Contains(x.order.WarehouseId.Value));
        }

        var payments = await paymentsQuery
            .Select(x => new { x.payment.PaymentDateUtc, x.payment.Amount })
            .ToListAsync(cancellationToken);

        var shipmentChargeQuery = _db.ShipmentCharges
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .Join(_db.Shipments.AsNoTracking().Where(x => !x.IsDeleted), charge => charge.ShipmentId, shipment => shipment.Id, (charge, shipment) => new { charge, shipment })
            .Where(x => x.charge.CreatedAtUtc >= fromDate);

        if (HasWarehouseFilter(warehouseIds))
        {
            shipmentChargeQuery = shipmentChargeQuery.Where(x => warehouseIds.Contains(x.shipment.WarehouseId));
        }

        var shipmentCharges = await shipmentChargeQuery
            .Select(x => new { x.charge.CreatedAtUtc, x.charge.Amount, ChargeType = x.charge.ChargeType.ToString() })
            .ToListAsync(cancellationToken);

        var productionCostQuery = _db.ProductionOrders.AsNoTracking().Where(x => !x.IsDeleted && x.CreatedAtUtc >= fromDate);
        if (HasWarehouseFilter(warehouseIds))
        {
            productionCostQuery = productionCostQuery.Where(x => warehouseIds.Contains(x.WarehouseId));
        }

        var productionCosts = await productionCostQuery
            .Select(x => new
            {
                x.CreatedAtUtc,
                Cost = x.ActualMaterialCost + x.ActualLaborCost + x.ActualOverheadCost
            })
            .ToListAsync(cancellationToken);

        var expenseByMonth = shipmentCharges
            .Select(x => new { x.CreatedAtUtc, Amount = x.Amount })
            .Concat(productionCosts.Select(x => new { x.CreatedAtUtc, Amount = x.Cost }))
            .GroupBy(x => new { x.CreatedAtUtc.Year, x.CreatedAtUtc.Month })
            .ToDictionary(x => $"{x.Key.Year}-{x.Key.Month:00}", x => x.Sum(y => y.Amount));

        var revenueByMonth = payments
            .GroupBy(x => new { x.PaymentDateUtc.Year, x.PaymentDateUtc.Month })
            .ToDictionary(x => $"{x.Key.Year}-{x.Key.Month:00}", x => x.Sum(y => y.Amount));

        var allMonthLabels = revenueByMonth.Keys
            .Union(expenseByMonth.Keys)
            .OrderBy(x => x)
            .ToList();

        var revenueExpenseTrend = allMonthLabels
            .Select(label => new MultiSeriesPointReadModel
            {
                Label = label,
                Series1 = revenueByMonth.GetValueOrDefault(label, 0m),
                Series2 = expenseByMonth.GetValueOrDefault(label, 0m)
            })
            .ToList();

        var expenseBreakdown = shipmentCharges
            .GroupBy(x => x.ChargeType)
            .Select(x => new PieSliceReadModel
            {
                Label = x.Key,
                Value = x.Sum(y => y.Amount)
            })
            .OrderByDescending(x => x.Value)
            .ToList();

        if (productionCosts.Count > 0)
        {
            expenseBreakdown.Add(new PieSliceReadModel
            {
                Label = "Production",
                Value = productionCosts.Sum(x => x.Cost)
            });
        }

        return new FinanceAnalyticsReadModel
        {
            RevenueExpenseTrend = revenueExpenseTrend,
            ExpenseBreakdown = expenseBreakdown
        };
    }

    private async Task<InventoryAnalyticsReadModel> GetInventoryAnalyticsAsync(
        DateTime fromDate,
        List<Guid> warehouseIds,
        CancellationToken cancellationToken)
    {
        var lowStockItems = await GetLowStockBaseQuery(warehouseIds)
            .OrderBy(x => x.Stock.QuantityAvailable - x.Product.ReorderLevel)
            .Take(5)
            .Select(x => new ChartPointReadModel
            {
                Label = x.Product.Name,
                Value = x.Stock.QuantityAvailable
            })
            .ToListAsync(cancellationToken);

        var movementQuery = _db.StockMovements.AsNoTracking().Where(x => x.MovementDateUtc >= fromDate);
        movementQuery = movementQuery.Where(x => !x.IsDeleted);
        if (HasWarehouseFilter(warehouseIds))
        {
            movementQuery = movementQuery.Where(x => warehouseIds.Contains(x.WarehouseId));
        }

        var movements = await movementQuery
            .Select(x => new { x.MovementDateUtc, x.MovementType, x.Quantity })
            .ToListAsync(cancellationToken);

        var inflowOutflow = movements
            .GroupBy(x => new { x.MovementDateUtc.Year, x.MovementDateUtc.Month })
            .OrderBy(x => x.Key.Year).ThenBy(x => x.Key.Month)
            .Select(x => new MultiSeriesPointReadModel
            {
                Label = $"{x.Key.Year}-{x.Key.Month:00}",
                Series1 = x.Where(y =>
                        y.MovementType == StockMovementType.StockIn ||
                        y.MovementType == StockMovementType.AdjustmentIncrease ||
                        y.MovementType == StockMovementType.TransferIn ||
                        y.MovementType == StockMovementType.ReturnIn)
                    .Sum(y => y.Quantity),
                Series2 = x.Where(y =>
                        y.MovementType == StockMovementType.StockOut ||
                        y.MovementType == StockMovementType.AdjustmentDecrease ||
                        y.MovementType == StockMovementType.TransferOut ||
                        y.MovementType == StockMovementType.ReturnOut ||
                        y.MovementType == StockMovementType.Damaged ||
                        y.MovementType == StockMovementType.Expired)
                    .Sum(y => y.Quantity)
            })
            .ToList();

        var stockRows = await GetInventoryBaseQuery(warehouseIds)
            .Select(x => new
            {
                Warehouse = x.Stock.Warehouse.Name,
                Category = x.Product.Category.Name,
                x.Stock.QuantityOnHand
            })
            .ToListAsync(cancellationToken);

        var warehouseStockComposition = stockRows
            .GroupBy(x => x.Warehouse)
            .Select(x => new WarehouseStockCompositionReadModel
            {
                Warehouse = x.Key,
                RawMaterials = x.Where(y => y.Category.Contains("raw", StringComparison.OrdinalIgnoreCase)).Sum(y => y.QuantityOnHand),
                FinishedGoods = x.Where(y => y.Category.Contains("finished", StringComparison.OrdinalIgnoreCase)).Sum(y => y.QuantityOnHand),
                Packaging = x.Where(y => y.Category.Contains("pack", StringComparison.OrdinalIgnoreCase)).Sum(y => y.QuantityOnHand)
            })
            .OrderBy(x => x.Warehouse)
            .ToList();

        var inventoryMixByCategory = stockRows
            .GroupBy(x => x.Category)
            .Select(x => new PieSliceReadModel
            {
                Label = x.Key,
                Value = x.Sum(y => y.QuantityOnHand)
            })
            .OrderByDescending(x => x.Value)
            .ToList();

        return new InventoryAnalyticsReadModel
        {
            TopLowStockItems = lowStockItems,
            InventoryInflowOutflow = inflowOutflow,
            WarehouseStockComposition = warehouseStockComposition,
            InventoryMixByCategory = inventoryMixByCategory
        };
    }

    private async Task<ProductionAnalyticsReadModel> GetProductionAnalyticsAsync(
        DateTime fromDate,
        List<Guid> warehouseIds,
        CancellationToken cancellationToken)
    {
        var productionOrdersQuery = _db.ProductionOrders.AsNoTracking().Where(x => !x.IsDeleted && x.PlannedStartDate >= fromDate);
        if (HasWarehouseFilter(warehouseIds))
        {
            productionOrdersQuery = productionOrdersQuery.Where(x => warehouseIds.Contains(x.WarehouseId));
        }

        var productionOrders = await productionOrdersQuery
            .Select(x => new
            {
                x.PlannedStartDate,
                x.PlannedQuantity,
                x.ProducedQuantity,
                Status = x.Status.ToString()
            })
            .ToListAsync(cancellationToken);

        var plannedVsActual = productionOrders
            .GroupBy(x => new { x.PlannedStartDate.Year, x.PlannedStartDate.Month })
            .OrderBy(x => x.Key.Year).ThenBy(x => x.Key.Month)
            .Select(x => new MultiSeriesPointReadModel
            {
                Label = $"{x.Key.Year}-{x.Key.Month:00}",
                Series1 = x.Sum(y => y.PlannedQuantity),
                Series2 = x.Sum(y => y.ProducedQuantity)
            })
            .ToList();

        var productionJobStatusMix = productionOrders
            .GroupBy(x => x.Status)
            .Select(x => new PieSliceReadModel
            {
                Label = x.Key,
                Value = x.Count()
            })
            .OrderByDescending(x => x.Value)
            .ToList();

        var executionQuery = _db.ProductionExecutions
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .Join(_db.WorkCenters.AsNoTracking().Where(x => !x.IsDeleted), execution => execution.WorkCenterId, workCenter => workCenter.Id, (execution, workCenter) => new { execution, workCenter })
            .Where(x => x.execution.PlannedStartDate >= fromDate);

        if (HasWarehouseFilter(warehouseIds))
        {
            executionQuery = executionQuery.Where(x => warehouseIds.Contains(x.workCenter.WarehouseId));
        }

        var executionRows = await executionQuery
            .Select(x => new
            {
                WorkCenter = x.workCenter.Name,
                x.execution.PlannedStartDate,
                x.execution.PlannedQuantity,
                x.execution.CompletedQuantity
            })
            .ToListAsync(cancellationToken);

        var productionLineEfficiency = executionRows
            .GroupBy(x => x.WorkCenter)
            .OrderBy(x => x.Key)
            .Select(x => new ProductionEfficiencySeriesReadModel
            {
                Line = x.Key,
                Points = x.GroupBy(y => new { y.PlannedStartDate.Year, y.PlannedStartDate.Month })
                    .OrderBy(y => y.Key.Year).ThenBy(y => y.Key.Month)
                    .Select(y => new ChartPointReadModel
                    {
                        Label = $"{y.Key.Year}-{y.Key.Month:00}",
                        Value = y.Sum(z => z.PlannedQuantity) <= 0m
                            ? 0m
                            : y.Sum(z => z.CompletedQuantity) / y.Sum(z => z.PlannedQuantity) * 100m
                    })
                    .ToList()
            })
            .ToList();

        return new ProductionAnalyticsReadModel
        {
            PlannedVsActualOutput = plannedVsActual,
            ProductionJobStatusMix = productionJobStatusMix,
            ProductionLineEfficiency = productionLineEfficiency
        };
    }

    private async Task<ShipmentAnalyticsReadModel> GetShipmentAnalyticsAsync(
        DateTime fromDate,
        List<Guid> warehouseIds,
        CancellationToken cancellationToken)
    {
        var shipmentQuery = _db.Shipments.AsNoTracking().Where(x => !x.IsDeleted && x.CreatedAtUtc >= fromDate);
        if (HasWarehouseFilter(warehouseIds))
        {
            shipmentQuery = shipmentQuery.Where(x => warehouseIds.Contains(x.WarehouseId));
        }

        var shipments = await shipmentQuery
            .Select(x => new
            {
                x.CreatedAtUtc,
                Status = x.Status.ToString(),
                x.PlannedDeliveryDateUtc,
                x.ActualDeliveryDateUtc
            })
            .ToListAsync(cancellationToken);

        var onTimeVsDelayedTrend = shipments
            .GroupBy(x => new { x.CreatedAtUtc.Year, x.CreatedAtUtc.Month })
            .OrderBy(x => x.Key.Year).ThenBy(x => x.Key.Month)
            .Select(x => new MultiSeriesPointReadModel
            {
                Label = $"{x.Key.Year}-{x.Key.Month:00}",
                Series1 = x.Count(y => y.ActualDeliveryDateUtc.HasValue &&
                                       y.PlannedDeliveryDateUtc.HasValue &&
                                       y.ActualDeliveryDateUtc <= y.PlannedDeliveryDateUtc),
                Series2 = x.Count(y => y.ActualDeliveryDateUtc.HasValue &&
                                       y.PlannedDeliveryDateUtc.HasValue &&
                                       y.ActualDeliveryDateUtc > y.PlannedDeliveryDateUtc)
            })
            .ToList();

        var shipmentStatusDistribution = shipments
            .GroupBy(x => x.Status)
            .Select(x => new PieSliceReadModel
            {
                Label = x.Key,
                Value = x.Count()
            })
            .OrderByDescending(x => x.Value)
            .ToList();

        return new ShipmentAnalyticsReadModel
        {
            OnTimeVsDelayedTrend = onTimeVsDelayedTrend,
            ShipmentStatusDistribution = shipmentStatusDistribution
        };
    }

    private async Task<List<WorkflowPipelineReadModel>> GetWorkflowPipelineAsync(
        DateTime fromDate,
        List<Guid> warehouseIds,
        CancellationToken cancellationToken)
    {
        var orderQuery = _db.Orders.AsNoTracking().Where(x => x.OrderDateUtc >= fromDate);
        if (HasWarehouseFilter(warehouseIds))
        {
            orderQuery = orderQuery.Where(x => x.WarehouseId.HasValue && warehouseIds.Contains(x.WarehouseId.Value));
        }

        var productionQuery = _db.ProductionOrders.AsNoTracking().Where(x => !x.IsDeleted && x.CreatedAtUtc >= fromDate);
        if (HasWarehouseFilter(warehouseIds))
        {
            productionQuery = productionQuery.Where(x => warehouseIds.Contains(x.WarehouseId));
        }

        var shipmentQuery = _db.Shipments.AsNoTracking().Where(x => !x.IsDeleted && x.CreatedAtUtc >= fromDate);
        if (HasWarehouseFilter(warehouseIds))
        {
            shipmentQuery = shipmentQuery.Where(x => warehouseIds.Contains(x.WarehouseId));
        }

        var orderBacklog = await orderQuery.CountAsync(
            x => x.Status == OrderStatus.PendingApproval || x.Status == OrderStatus.Processing,
            cancellationToken);
        var productionActive = await productionQuery.CountAsync(
            x => x.Status == ProductionOrderStatus.Released || x.Status == ProductionOrderStatus.InProgress,
            cancellationToken);
        var shipmentActive = await shipmentQuery.CountAsync(
            x => x.Status == ShipmentStatus.Picking ||
                 x.Status == ShipmentStatus.Packed ||
                 x.Status == ShipmentStatus.ReadyToDispatch ||
                 x.Status == ShipmentStatus.InTransit,
            cancellationToken);

        var max = Math.Max(1, Math.Max(orderBacklog, Math.Max(productionActive, shipmentActive)));

        return
        [
            new WorkflowPipelineReadModel
            {
                Label = "Order backlog",
                Count = orderBacklog,
                Progress = Math.Round((decimal)orderBacklog / max * 100m, 2)
            },
            new WorkflowPipelineReadModel
            {
                Label = "Production active",
                Count = productionActive,
                Progress = Math.Round((decimal)productionActive / max * 100m, 2)
            },
            new WorkflowPipelineReadModel
            {
                Label = "Shipments active",
                Count = shipmentActive,
                Progress = Math.Round((decimal)shipmentActive / max * 100m, 2)
            }
        ];
    }

    private async Task<List<ActivityFeedReadModel>> GetActivityFeedAsync(
        DateTime fromDate,
        List<Guid> warehouseIds,
        CancellationToken cancellationToken)
    {
        var orderQuery = _db.Orders.AsNoTracking().Where(x => x.CreatedAtUtc >= fromDate);
        if (HasWarehouseFilter(warehouseIds))
        {
            orderQuery = orderQuery.Where(x => x.WarehouseId.HasValue && warehouseIds.Contains(x.WarehouseId.Value));
        }

        var orderEvents = await orderQuery
            .OrderByDescending(x => x.CreatedAtUtc)
            .Take(5)
            .Select(x => new ActivityFeedReadModel
            {
                Text = $"Order {x.OrderNumber} created for {x.CustomerName ?? "customer"}",
                TimeUtc = x.CreatedAtUtc,
                Type = "Order"
            })
            .ToListAsync(cancellationToken);

        var shipmentQuery = _db.Shipments.AsNoTracking().Where(x => !x.IsDeleted && x.CreatedAtUtc >= fromDate);
        if (HasWarehouseFilter(warehouseIds))
        {
            shipmentQuery = shipmentQuery.Where(x => warehouseIds.Contains(x.WarehouseId));
        }

        var shipmentEvents = await shipmentQuery
            .OrderByDescending(x => x.CreatedAtUtc)
            .Take(5)
            .Select(x => new ActivityFeedReadModel
            {
                Text = $"Shipment {x.ShipmentNumber} is {x.Status}",
                TimeUtc = x.CreatedAtUtc,
                Type = "Shipment"
            })
            .ToListAsync(cancellationToken);

        var productionQuery = _db.ProductionOrders.AsNoTracking().Where(x => !x.IsDeleted && x.CreatedAtUtc >= fromDate);
        if (HasWarehouseFilter(warehouseIds))
        {
            productionQuery = productionQuery.Where(x => warehouseIds.Contains(x.WarehouseId));
        }

        var productionEvents = await productionQuery
            .OrderByDescending(x => x.CreatedAtUtc)
            .Take(5)
            .Select(x => new ActivityFeedReadModel
            {
                Text = $"Production order {x.ProductionOrderNumber} is {x.Status}",
                TimeUtc = x.CreatedAtUtc,
                Type = "Production"
            })
            .ToListAsync(cancellationToken);

        return orderEvents
            .Concat(shipmentEvents)
            .Concat(productionEvents)
            .OrderByDescending(x => x.TimeUtc)
            .Take(10)
            .ToList();
    }

    private async Task<WorkforceSummaryReadModel> GetWorkforceSummaryAsync(CancellationToken cancellationToken)
    {
        var activeStaff = await _db.Users.AsNoTracking().CountAsync(x => x.IsActive && !x.IsDeleted, cancellationToken);
        var activeWorkCenters = await _db.WorkCenters
            .AsNoTracking()
            .CountAsync(x => x.IsActive && !x.IsDeleted, cancellationToken);
        var operatorCapacity = await _db.WorkCenters
            .AsNoTracking()
            .Where(x => x.IsActive && !x.IsDeleted)
            .SumAsync(x => (int?)x.AvailableOperators, cancellationToken) ?? 0;
        var targetOperators = activeWorkCenters * 2;
        var shiftCoverage = targetOperators == 0
            ? 0m
            : Math.Round((decimal)operatorCapacity / targetOperators * 100m, 2);

        return new WorkforceSummaryReadModel
        {
            ActiveStaff = activeStaff,
            ShiftCoverage = shiftCoverage,
            OpenPositions = Math.Max(0, targetOperators - operatorCapacity)
        };
    }

    private async Task<ProcurementSummaryReadModel> GetProcurementSummaryAsync(
        DateTime fromDate,
        List<Guid> warehouseIds,
        CancellationToken cancellationToken)
    {
        var purchaseOrdersQuery = _db.Orders
            .AsNoTracking()
            .Where(x => x.OrderType == OrderType.Purchase && x.OrderDateUtc >= fromDate);

        if (HasWarehouseFilter(warehouseIds))
        {
            purchaseOrdersQuery = purchaseOrdersQuery.Where(x => x.WarehouseId.HasValue && warehouseIds.Contains(x.WarehouseId.Value));
        }

        var purchaseOrders = await purchaseOrdersQuery
            .Select(x => new { x.Status, x.RequiredDateUtc, x.FulfilledDateUtc })
            .ToListAsync(cancellationToken);

        var awaitingApproval = purchaseOrders.Count(x => x.Status == OrderStatus.PendingApproval);
        var openPurchaseOrders = purchaseOrders.Count(x =>
            x.Status == OrderStatus.PendingApproval ||
            x.Status == OrderStatus.Approved ||
            x.Status == OrderStatus.Processing ||
            x.Status == OrderStatus.PartiallyFulfilled);

        var measured = purchaseOrders.Count(x => x.RequiredDateUtc.HasValue && x.FulfilledDateUtc.HasValue);
        var slaMet = measured == 0
            ? 0m
            : (decimal)purchaseOrders.Count(x => x.RequiredDateUtc.HasValue &&
                                                 x.FulfilledDateUtc.HasValue &&
                                                 x.FulfilledDateUtc <= x.RequiredDateUtc) / measured * 100m;

        return new ProcurementSummaryReadModel
        {
            OpenPurchaseOrders = openPurchaseOrders,
            AwaitingApproval = awaitingApproval,
            SupplierSlaMet = Math.Round(slaMet, 2)
        };
    }

    private async Task<WarehouseSummaryReadModel> GetWarehouseSummaryAsync(
        DateTime fromDate,
        List<Guid> warehouseIds,
        CancellationToken cancellationToken)
    {
        var warehouseQuery = _db.Warehouses.AsNoTracking().Where(x => x.IsActive && !x.IsDeleted);
        if (HasWarehouseFilter(warehouseIds))
        {
            warehouseQuery = warehouseQuery.Where(x => warehouseIds.Contains(x.Id));
        }

        var warehousesActive = await warehouseQuery.CountAsync(cancellationToken);

        var shipmentQuery = _db.Shipments.AsNoTracking().Where(x => !x.IsDeleted && x.CreatedAtUtc >= fromDate);
        if (HasWarehouseFilter(warehouseIds))
        {
            shipmentQuery = shipmentQuery.Where(x => warehouseIds.Contains(x.WarehouseId));
        }

        var shipmentRows = await shipmentQuery
            .Select(x => new { x.Type, x.Status })
            .ToListAsync(cancellationToken);

        var delivered = shipmentRows.Count(x => x.Status == ShipmentStatus.Delivered);
        var measurable = shipmentRows.Count(x =>
            x.Status == ShipmentStatus.Delivered ||
            x.Status == ShipmentStatus.DeliveryFailed ||
            x.Status == ShipmentStatus.Returned);
        var crossDockCount = shipmentRows.Count(x => x.Type == ShipmentType.Transfer);

        return new WarehouseSummaryReadModel
        {
            WarehousesActive = warehousesActive,
            AveragePickAccuracy = measurable == 0 ? 0m : Math.Round((decimal)delivered / measurable * 100m, 2),
            CrossDockUtilization = shipmentRows.Count == 0 ? 0m : Math.Round((decimal)crossDockCount / shipmentRows.Count * 100m, 2)
        };
    }

    private async Task<List<RecentOrderReadModel>> GetRecentOrdersAsync(
        DateTime fromDate,
        List<Guid> warehouseIds,
        CancellationToken cancellationToken)
    {
        var ordersQuery = _db.Orders
            .AsNoTracking()
            .Include(x => x.Warehouse)
            .Where(x => x.OrderDateUtc >= fromDate);

        if (HasWarehouseFilter(warehouseIds))
        {
            ordersQuery = ordersQuery.Where(x => x.WarehouseId.HasValue && warehouseIds.Contains(x.WarehouseId.Value));
        }

        return await ordersQuery
            .OrderByDescending(x => x.OrderDateUtc)
            .Take(10)
            .Select(x => new RecentOrderReadModel
            {
                OrderNo = x.OrderNumber,
                Customer = x.CustomerName ?? "Unknown",
                Module = x.OrderType.ToString(),
                Amount = x.TotalAmount,
                Status = x.Status.ToString(),
                DueDate = x.RequiredDateUtc ?? x.OrderDateUtc,
                Warehouse = x.Warehouse != null ? x.Warehouse.Name : "N/A"
            })
            .ToListAsync(cancellationToken);
    }

    private async Task<List<LowStockItemReadModel>> GetLowStockItemsAsync(
        List<Guid> warehouseIds,
        CancellationToken cancellationToken)
    {
        return await GetLowStockBaseQuery(warehouseIds)
            .OrderBy(x => x.Stock.QuantityAvailable - x.Product.ReorderLevel)
            .Take(10)
            .Select(x => new LowStockItemReadModel
            {
                Sku = x.Product.SKU,
                Item = x.Product.Name,
                Warehouse = x.Stock.Warehouse.Name,
                Stock = x.Stock.QuantityAvailable,
                ReorderLevel = x.Product.ReorderLevel,
                Status = x.Stock.QuantityAvailable <= 0m ? "Out of stock" : "Low stock"
            })
            .ToListAsync(cancellationToken);
    }

    private IQueryable<InventoryDashboardRow> GetInventoryBaseQuery(List<Guid> warehouseIds)
    {
        var query = _db.InventoryStocks
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .Join(
                _db.Products.AsNoTracking().Where(x => !x.IsDeleted),
                stock => stock.ProductId,
                product => product.Id,
                (stock, product) => new InventoryDashboardRow
                {
                    Stock = stock,
                    Product = product
                });

        if (HasWarehouseFilter(warehouseIds))
        {
            query = query.Where(x => warehouseIds.Contains(x.Stock.WarehouseId));
        }

        return query;
    }

    private IQueryable<InventoryDashboardRow> GetLowStockBaseQuery(List<Guid> warehouseIds)
    {
        return GetInventoryBaseQuery(warehouseIds)
            .Where(x => x.Stock.QuantityAvailable <= x.Product.ReorderLevel);
    }
}
