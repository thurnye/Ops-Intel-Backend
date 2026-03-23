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
        DateOnly? from,
        DateOnly? to,
        string site,
        CancellationToken cancellationToken = default)
    {
        var anchorDate = await ResolveDashboardAnchorDateAsync(cancellationToken);

        var toDate = ResolveToDate(to, anchorDate);
        var fromDate = ResolveFromDate(from, toDate);
        var previousFromDate = ResolvePreviousFromDate(fromDate, toDate);

        var warehouseIds = await ResolveWarehouseIdsAsync(site, cancellationToken);

        var currentKpis = await GetKpisAsync(fromDate, toDate, warehouseIds, cancellationToken);
        var previousKpis = await GetKpisAsync(previousFromDate, fromDate, warehouseIds, cancellationToken);

        return new DashboardOverviewReadModel
        {
            Kpis = currentKpis,
            KpiComparison = GetKpiComparison(currentKpis, previousKpis),

            Alerts = await GetAlertsAsync(fromDate, toDate, warehouseIds, cancellationToken),
            ModuleHealth = await GetModuleHealthAsync(fromDate, toDate, warehouseIds, cancellationToken),

            BusinessPerformance = await GetBusinessPerformanceAsync(fromDate, toDate, warehouseIds, cancellationToken),
            Finance = await GetFinanceAnalyticsAsync(fromDate, toDate, warehouseIds, cancellationToken),
            Inventory = await GetInventoryAnalyticsAsync(fromDate, toDate, warehouseIds, cancellationToken),
            Production = await GetProductionAnalyticsAsync(fromDate, toDate, warehouseIds, cancellationToken),
            Shipment = await GetShipmentAnalyticsAsync(fromDate, toDate, warehouseIds, cancellationToken),

            Operations = await GetOperationsAsync(fromDate, toDate, warehouseIds, cancellationToken),

            WorkflowPipeline = await GetWorkflowPipelineAsync(fromDate, toDate, warehouseIds, cancellationToken),
            ActivityFeed = await GetActivityFeedAsync(fromDate, toDate, warehouseIds, cancellationToken),

            WorkforceSummary = await GetWorkforceSummaryAsync(cancellationToken),
            ProcurementSummary = await GetProcurementSummaryAsync(fromDate, toDate, warehouseIds, cancellationToken),
            WarehouseSummary = await GetWarehouseSummaryAsync(fromDate, toDate, warehouseIds, cancellationToken),

            RecentOrders = await GetRecentOrdersAsync(fromDate, toDate, warehouseIds, cancellationToken),
            LowStockItems = await GetLowStockItemsAsync(warehouseIds, cancellationToken)
        };
    }

    private async Task<DateTime> ResolveDashboardAnchorDateAsync(CancellationToken cancellationToken)
    {
        var invoiceMax = await _db.Invoices
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .MaxAsync(x => (DateTime?)x.InvoiceDate, cancellationToken);

        var paymentMax = await _db.Payments
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .MaxAsync(x => (DateTime?)x.PaymentDate, cancellationToken);

        var orderMax = await _db.Orders
            .AsNoTracking()
            .Where(x => x.IsActive)
            .MaxAsync(x => (DateTime?)x.OrderDateUtc, cancellationToken);

        var shipmentMax = await _db.Shipments
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .MaxAsync(x => (DateTime?)x.CreatedAtUtc, cancellationToken);

        var productionMax = await _db.ProductionOrders
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .MaxAsync(x => (DateTime?)x.CreatedAtUtc, cancellationToken);

        return new[] { invoiceMax, paymentMax, orderMax, shipmentMax, productionMax }
            .Where(x => x.HasValue)
            .Select(x => x!.Value)
            .DefaultIfEmpty(DateTime.UtcNow)
            .Max();
    }

    private static DateTime ResolveToDate(DateOnly? to, DateTime anchorDate)
    {
        return to.HasValue ? to.Value.ToDateTime(TimeOnly.MaxValue) : anchorDate;
    }

    private static DateTime ResolveFromDate(DateOnly? from, DateTime toDate)
    {
        return from.HasValue ? from.Value.ToDateTime(TimeOnly.MinValue) : toDate.AddDays(-30);
    }

    private static DateTime ResolvePreviousFromDate(DateTime fromDate, DateTime toDate)
    {
        var span = toDate - fromDate;
        return fromDate - span;
    }

    private static bool HasWarehouseFilter(IReadOnlyCollection<Guid> warehouseIds)
        => warehouseIds.Count > 0;

    private async Task<List<Guid>> ResolveWarehouseIdsAsync(
        string? site,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(site) || site.Equals("all", StringComparison.OrdinalIgnoreCase))
            return [];

        var normalized = site.Trim().ToLowerInvariant();

        return await _db.Warehouses
            .AsNoTracking()
            .Where(x => x.IsActive && !x.IsDeleted)
            .Where(x =>
                x.Name.ToLower().Contains(normalized) ||
                x.Code.ToLower().Contains(normalized) ||
                (x.City != null && x.City.ToLower().Contains(normalized)))
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    private static DashboardKpiComparisonReadModel GetKpiComparison(
        DashboardKpisReadModel current,
        DashboardKpisReadModel previous)
    {
        return new DashboardKpiComparisonReadModel
        {
            RevenueChangePercent = CalculatePercentChange(previous.TotalRevenue, current.TotalRevenue),
            OrdersInProgressChangePercent = CalculatePercentChange(previous.OrdersInProgress, current.OrdersInProgress),
            ProductionEfficiencyChangePercent = CalculatePercentChange(previous.ProductionEfficiency, current.ProductionEfficiency),
            InventoryValueChangePercent = CalculatePercentChange(previous.InventoryValue, current.InventoryValue),
            ShipmentsPendingChangePercent = CalculatePercentChange(previous.ShipmentsPending, current.ShipmentsPending),
            CriticalAlertsChangePercent = CalculatePercentChange(previous.CriticalAlerts, current.CriticalAlerts)
        };
    }

    private static decimal CalculatePercentChange(decimal previous, decimal current)
    {
        if (previous == 0m)
            return current == 0m ? 0m : 100m;

        return ((current - previous) / previous) * 100m;
    }

    private static decimal CalculatePercentChange(int previous, int current)
    {
        if (previous == 0)
            return current == 0 ? 0m : 100m;

        return ((decimal)(current - previous) / previous) * 100m;
    }

    private static List<DateTime> GetMonthBuckets(DateTime fromDate, DateTime toDate)
    {
        var start = new DateTime(fromDate.Year, fromDate.Month, 1);
        var end = new DateTime(toDate.Year, toDate.Month, 1);

        var buckets = new List<DateTime>();
        var current = start;

        while (current <= end)
        {
            buckets.Add(current);
            current = current.AddMonths(1);
        }

        return buckets;
    }

    private static string ToMonthKey(DateTime date)
    {
        return $"{date.Year}-{date.Month:00}";
    }

    private static string ToMonthLabel(DateTime date)
    {
        return date.ToString("MMM");
    }

    private static DateTime GetTwelveMonthTrendStart(DateTime toDate)
    {
        var monthStart = new DateTime(toDate.Year, toDate.Month, 1);
        return monthStart.AddMonths(-11);
    }

    private async Task<DashboardKpisReadModel> GetKpisAsync(
        DateTime fromDate,
        DateTime toDate,
        List<Guid> warehouseIds,
        CancellationToken cancellationToken)
    {
        var hasWarehouseFilter = HasWarehouseFilter(warehouseIds);

        var invoiceQuery = _db.Invoices
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.InvoiceDate >= fromDate && x.InvoiceDate < toDate);

        var totalRevenue = await invoiceQuery
            .SumAsync(x => (decimal?)x.TotalAmount, cancellationToken) ?? 0m;

        var ordersQuery = _db.Orders
            .AsNoTracking()
            .Where(x => x.IsActive && x.OrderDateUtc >= fromDate && x.OrderDateUtc < toDate);

        if (hasWarehouseFilter)
        {
            ordersQuery = ordersQuery.Where(x =>
                x.WarehouseId.HasValue &&
                warehouseIds.Contains(x.WarehouseId.Value));
        }

        var ordersInProgress = await ordersQuery.CountAsync(
            x => x.Status == OrderStatus.PendingApproval ||
                 x.Status == OrderStatus.Approved ||
                 x.Status == OrderStatus.Processing ||
                 x.Status == OrderStatus.PartiallyFulfilled,
            cancellationToken);

        var inventoryValue = await GetInventoryBaseQuery(warehouseIds)
            .SumAsync(x => (decimal?)x.Stock.QuantityOnHand * x.Product.CostPrice, cancellationToken) ?? 0m;

        var shipmentsQuery = _db.Shipments
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.CreatedAtUtc >= fromDate && x.CreatedAtUtc < toDate);

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

        var unresolvedExceptions = await _db.ShipmentExceptions
            .AsNoTracking()
            .Where(x => !x.IsDeleted && !x.IsResolved && x.ReportedAtUtc >= fromDate && x.ReportedAtUtc < toDate)
            .CountAsync(cancellationToken);

        var executionQuery = _db.ProductionExecutions
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.CreatedAtUtc >= fromDate && x.CreatedAtUtc < toDate);

        var executionRatios = await executionQuery
            .Select(x => x.PlannedQuantity > 0m
                ? (x.CompletedQuantity / x.PlannedQuantity) * 100m
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
        DateTime toDate,
        List<Guid> warehouseIds,
        CancellationToken cancellationToken)
    {
        var lowStockCount = await GetLowStockBaseQuery(warehouseIds).CountAsync(cancellationToken);

        var delayedShipmentQuery = _db.ShipmentExceptions
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.ReportedAtUtc >= fromDate && x.ReportedAtUtc < toDate)
            .Where(x => x.ExceptionType == ShipmentExceptionType.Delay)
            .Join(
                _db.Shipments.AsNoTracking().Where(x => !x.IsDeleted),
                ex => ex.ShipmentId,
                shipment => shipment.Id,
                (ex, shipment) => new { ex, shipment });

        if (HasWarehouseFilter(warehouseIds))
        {
            delayedShipmentQuery = delayedShipmentQuery.Where(x => warehouseIds.Contains(x.shipment.WarehouseId));
        }

        var delayedShipments = await delayedShipmentQuery.CountAsync(cancellationToken);

        var pendingOrdersQuery = _db.Orders
            .AsNoTracking()
            .Where(x => x.IsActive && x.OrderDateUtc >= fromDate && x.OrderDateUtc < toDate)
            .Where(x => x.Status == OrderStatus.PendingApproval);

        if (HasWarehouseFilter(warehouseIds))
        {
            pendingOrdersQuery = pendingOrdersQuery.Where(x =>
                x.WarehouseId.HasValue &&
                warehouseIds.Contains(x.WarehouseId.Value));
        }

        var pendingApprovals = await pendingOrdersQuery.CountAsync(cancellationToken);

        var underTargetQuery = _db.ProductionExecutions
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.CreatedAtUtc >= fromDate && x.CreatedAtUtc < toDate)
            .Where(x => x.PlannedQuantity > 0m && x.CompletedQuantity < x.PlannedQuantity);

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
        DateTime toDate,
        List<Guid> warehouseIds,
        CancellationToken cancellationToken)
    {
        var lowStockCount = await GetLowStockBaseQuery(warehouseIds).CountAsync(cancellationToken);

        var qualityQuery = _db.ProductionQualityChecks
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.CheckDate >= fromDate && x.CheckDate < toDate)
            .Where(x => x.Status == QualityCheckStatus.Failed)
            .Join(
                _db.ProductionOrders.AsNoTracking().Where(x => !x.IsDeleted),
                qc => qc.ProductionOrderId,
                po => po.Id,
                (qc, po) => new { qc, po });

        if (HasWarehouseFilter(warehouseIds))
        {
            qualityQuery = qualityQuery.Where(x => warehouseIds.Contains(x.po.WarehouseId));
        }

        var failedQualityChecks = await qualityQuery.CountAsync(cancellationToken);

        var shipmentIssuesQuery = _db.ShipmentExceptions
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.ReportedAtUtc >= fromDate && x.ReportedAtUtc < toDate && !x.IsResolved)
            .Join(
                _db.Shipments.AsNoTracking().Where(x => !x.IsDeleted),
                ex => ex.ShipmentId,
                shipment => shipment.Id,
                (ex, shipment) => new { ex, shipment });

        if (HasWarehouseFilter(warehouseIds))
        {
            shipmentIssuesQuery = shipmentIssuesQuery.Where(x => warehouseIds.Contains(x.shipment.WarehouseId));
        }

        var shipmentIssues = await shipmentIssuesQuery.CountAsync(cancellationToken);

        var activeProductionQuery = _db.ProductionOrders
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.CreatedAtUtc >= fromDate && x.CreatedAtUtc < toDate);

        if (HasWarehouseFilter(warehouseIds))
        {
            activeProductionQuery = activeProductionQuery.Where(x => warehouseIds.Contains(x.WarehouseId));
        }

        var activeProductionJobs = await activeProductionQuery.CountAsync(
            x => x.Status == ProductionOrderStatus.Released ||
                 x.Status == ProductionOrderStatus.InProgress ||
                 x.Status == ProductionOrderStatus.Planned,
            cancellationToken);

        var receivables = await _db.AccountsReceivable
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.InvoiceDate >= fromDate && x.InvoiceDate < toDate)
            .SumAsync(x => (decimal?)x.OutstandingAmount, cancellationToken) ?? 0m;

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
                Module = "Production",
                Status = failedQualityChecks > 0 ? "Warning" : "Healthy",
                Value = activeProductionJobs.ToString(),
                Note = failedQualityChecks > 0
                    ? "Active jobs with failed quality checks detected"
                    : "Active jobs running within expected quality targets"
            },
            new DashboardModuleHealthReadModel
            {
                Module = "Shipment",
                Status = shipmentIssues > 0 ? "Warning" : "Healthy",
                Value = shipmentIssues.ToString(),
                Note = "Open shipment exceptions"
            },
            new DashboardModuleHealthReadModel
            {
                Module = "Finance",
                Status = receivables > 0m ? "Healthy" : "Warning",
                Value = receivables.ToString("0.##"),
                Note = "Outstanding receivables in selected range"
            }
        ];
    }

    private async Task<BusinessPerformanceReadModel> GetBusinessPerformanceAsync(
        DateTime fromDate,
        DateTime toDate,
        List<Guid> warehouseIds,
        CancellationToken cancellationToken)
    {
        var trendFromDate = GetTwelveMonthTrendStart(toDate);

        var invoices = await _db.Invoices
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.InvoiceDate >= trendFromDate && x.InvoiceDate < toDate)
            .Select(x => new
            {
                x.InvoiceDate,
                x.TotalAmount
            })
            .ToListAsync(cancellationToken);

        var revenueByMonth = invoices
            .GroupBy(x => ToMonthKey(x.InvoiceDate))
            .ToDictionary(x => x.Key, x => x.Sum(y => y.TotalAmount));

        var monthlyRevenueTrend = GetMonthBuckets(trendFromDate, toDate)
            .Select(month =>
            {
                var key = ToMonthKey(month);
                return new ChartPointReadModel
                {
                    Label = ToMonthLabel(month),
                    Value = revenueByMonth.GetValueOrDefault(key, 0m)
                };
            })
            .ToList();

        var shipmentQuery = _db.Shipments
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.CreatedAtUtc >= fromDate && x.CreatedAtUtc < toDate);

        if (HasWarehouseFilter(warehouseIds))
        {
            shipmentQuery = shipmentQuery.Where(x => warehouseIds.Contains(x.WarehouseId));
        }

        var deliveredShipments = await shipmentQuery
            .Where(x => x.ActualDeliveryDateUtc.HasValue && x.PlannedDeliveryDateUtc.HasValue)
            .Select(x => new { x.ActualDeliveryDateUtc, x.PlannedDeliveryDateUtc })
            .ToListAsync(cancellationToken);

        var inventorySnapshot = await _db.InventoryStocks
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .Where(x => !HasWarehouseFilter(warehouseIds) || warehouseIds.Contains(x.WarehouseId))
            .Select(x => new { x.QuantityOnHand, x.QuantityReserved })
            .ToListAsync(cancellationToken);

        var approvalQueueQuery = _db.Orders
            .AsNoTracking()
            .Where(x => x.IsActive && x.OrderDateUtc >= fromDate && x.OrderDateUtc < toDate)
            .Where(x => x.Status == OrderStatus.PendingApproval);

        if (HasWarehouseFilter(warehouseIds))
        {
            approvalQueueQuery = approvalQueueQuery.Where(x =>
                x.WarehouseId.HasValue &&
                warehouseIds.Contains(x.WarehouseId.Value));
        }

        var approvalQueue = await approvalQueueQuery.CountAsync(cancellationToken);

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
        DateTime toDate,
        List<Guid> warehouseIds,
        CancellationToken cancellationToken)
    {
        var invoices = await _db.Invoices
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.InvoiceDate >= fromDate && x.InvoiceDate < toDate)
            .Select(x => new
            {
                x.InvoiceDate,
                x.TotalAmount,
                x.AmountPaid,
                x.OutstandingAmount
            })
            .ToListAsync(cancellationToken);

        var payments = await _db.Payments
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.PaymentDate >= fromDate && x.PaymentDate < toDate)
            .Where(x => x.Status == PaymentStatus.Paid)
            .Select(x => new
            {
                x.PaymentDate,
                x.AmountReceived
            })
            .ToListAsync(cancellationToken);

        var expenses = await _db.Expenses
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.ExpenseDate >= fromDate && x.ExpenseDate < toDate)
            .Select(x => new
            {
                x.ExpenseDate,
                x.Category,
                x.Amount
            })
            .ToListAsync(cancellationToken);

        var revenueByMonth = invoices
            .GroupBy(x => ToMonthKey(x.InvoiceDate))
            .ToDictionary(x => x.Key, x => x.Sum(y => y.TotalAmount));

        var expenseByMonth = expenses
            .GroupBy(x => ToMonthKey(x.ExpenseDate))
            .ToDictionary(x => x.Key, x => x.Sum(y => y.Amount));

        var revenueExpenseTrend = GetMonthBuckets(fromDate, toDate)
            .Select(month =>
            {
                var key = ToMonthKey(month);
                var revenue = revenueByMonth.GetValueOrDefault(key, 0m);
                var expense = expenseByMonth.GetValueOrDefault(key, 0m);

                return new MultiSeriesPointReadModel
                {
                    Label = ToMonthLabel(month),
                    Series1 = revenue,
                    Series2 = expense,
                    Series3 = revenue - expense
                };
            })
            .ToList();

        var expenseBreakdown = expenses
            .GroupBy(x => x.Category)
            .Select(x => new PieSliceReadModel
            {
                Label = x.Key,
                Value = x.Sum(y => y.Amount)
            })
            .OrderByDescending(x => x.Value)
            .ToList();

        return new FinanceAnalyticsReadModel
        {
            RevenueExpenseTrend = revenueExpenseTrend,
            ExpenseBreakdown = expenseBreakdown
        };
    }

    private async Task<InventoryAnalyticsReadModel> GetInventoryAnalyticsAsync(
        DateTime fromDate,
        DateTime toDate,
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

        var movementQuery = _db.StockMovements
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.MovementDateUtc >= fromDate && x.MovementDateUtc < toDate);

        if (HasWarehouseFilter(warehouseIds))
        {
            movementQuery = movementQuery.Where(x => warehouseIds.Contains(x.WarehouseId));
        }

        var movements = await movementQuery
            .Select(x => new { x.MovementDateUtc, x.MovementType, x.Quantity })
            .ToListAsync(cancellationToken);

        var movementGrouped = movements
            .GroupBy(x => ToMonthKey(x.MovementDateUtc))
            .ToDictionary(
                x => x.Key,
                x => new
                {
                    Inflow = x.Where(y =>
                            y.MovementType == StockMovementType.StockIn ||
                            y.MovementType == StockMovementType.AdjustmentIncrease ||
                            y.MovementType == StockMovementType.TransferIn ||
                            y.MovementType == StockMovementType.ReturnIn)
                        .Sum(y => y.Quantity),
                    Outflow = x.Where(y =>
                            y.MovementType == StockMovementType.StockOut ||
                            y.MovementType == StockMovementType.AdjustmentDecrease ||
                            y.MovementType == StockMovementType.TransferOut ||
                            y.MovementType == StockMovementType.ReturnOut ||
                            y.MovementType == StockMovementType.Damaged ||
                            y.MovementType == StockMovementType.Expired)
                        .Sum(y => y.Quantity)
                });

        var inflowOutflow = GetMonthBuckets(fromDate, toDate)
            .Select(month =>
            {
                var key = ToMonthKey(month);
                var item = movementGrouped.GetValueOrDefault(key);

                return new MultiSeriesPointReadModel
                {
                    Label = ToMonthLabel(month),
                    Series1 = item?.Inflow ?? 0m,
                    Series2 = item?.Outflow ?? 0m
                };
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
        DateTime toDate,
        List<Guid> warehouseIds,
        CancellationToken cancellationToken)
    {
        var productionOrdersQuery = _db.ProductionOrders
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.PlannedStartDate >= fromDate && x.PlannedStartDate < toDate);

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

        var productionGrouped = productionOrders
            .GroupBy(x => ToMonthKey(x.PlannedStartDate))
            .ToDictionary(
                x => x.Key,
                x => new
                {
                    Planned = x.Sum(y => y.PlannedQuantity),
                    Actual = x.Sum(y => y.ProducedQuantity)
                });

        var plannedVsActual = GetMonthBuckets(fromDate, toDate)
            .Select(month =>
            {
                var key = ToMonthKey(month);
                var item = productionGrouped.GetValueOrDefault(key);

                return new MultiSeriesPointReadModel
                {
                    Label = ToMonthLabel(month),
                    Series1 = item?.Planned ?? 0m,
                    Series2 = item?.Actual ?? 0m
                };
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
            .Where(x => !x.IsDeleted && x.PlannedStartDate >= fromDate && x.PlannedStartDate < toDate)
            .Join(
                _db.WorkCenters.AsNoTracking().Where(x => !x.IsDeleted),
                execution => execution.WorkCenterId,
                workCenter => workCenter.Id,
                (execution, workCenter) => new { execution, workCenter });

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
                        Label = ToMonthLabel(new DateTime(y.Key.Year, y.Key.Month, 1)),
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
        DateTime toDate,
        List<Guid> warehouseIds,
        CancellationToken cancellationToken)
    {
        var shipmentQuery = _db.Shipments
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.CreatedAtUtc >= fromDate && x.CreatedAtUtc < toDate);

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

        var shipmentGrouped = shipments
            .GroupBy(x => ToMonthKey(x.CreatedAtUtc))
            .ToDictionary(
                x => x.Key,
                x => new
                {
                    OnTime = x.Count(y => y.ActualDeliveryDateUtc.HasValue &&
                                          y.PlannedDeliveryDateUtc.HasValue &&
                                          y.ActualDeliveryDateUtc <= y.PlannedDeliveryDateUtc),
                    Delayed = x.Count(y => y.ActualDeliveryDateUtc.HasValue &&
                                           y.PlannedDeliveryDateUtc.HasValue &&
                                           y.ActualDeliveryDateUtc > y.PlannedDeliveryDateUtc)
                });

        var onTimeVsDelayedTrend = GetMonthBuckets(fromDate, toDate)
            .Select(month =>
            {
                var key = ToMonthKey(month);
                var item = shipmentGrouped.GetValueOrDefault(key);

                return new MultiSeriesPointReadModel
                {
                    Label = ToMonthLabel(month),
                    Series1 = item?.OnTime ?? 0m,
                    Series2 = item?.Delayed ?? 0m
                };
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

    private async Task<DashboardOperationsReadModel> GetOperationsAsync(
        DateTime fromDate,
        DateTime toDate,
        List<Guid> warehouseIds,
        CancellationToken cancellationToken)
    {
        var hasWarehouseFilter = HasWarehouseFilter(warehouseIds);

        var ordersQuery = _db.Orders
            .AsNoTracking()
            .Where(x => x.IsActive && x.OrderDateUtc >= fromDate && x.OrderDateUtc < toDate);

        if (hasWarehouseFilter)
        {
            ordersQuery = ordersQuery.Where(x =>
                x.WarehouseId.HasValue &&
                warehouseIds.Contains(x.WarehouseId.Value));
        }

        var shipmentsQuery = _db.Shipments
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.CreatedAtUtc >= fromDate && x.CreatedAtUtc < toDate);

        if (hasWarehouseFilter)
        {
            shipmentsQuery = shipmentsQuery.Where(x => warehouseIds.Contains(x.WarehouseId));
        }

        var orderDays = await ordersQuery.Select(x => x.OrderDateUtc.DayOfWeek).ToListAsync(cancellationToken);
        var shipmentDays = await shipmentsQuery.Select(x => x.CreatedAtUtc.DayOfWeek).ToListAsync(cancellationToken);

        var weekdayOrder = new[]
        {
            DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday,
            DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday
        };

        var weeklyOrdersVsShipments = weekdayOrder
            .Select(day => new WeekdayMetricReadModel
            {
                Label = day switch
                {
                    DayOfWeek.Monday => "Mon",
                    DayOfWeek.Tuesday => "Tue",
                    DayOfWeek.Wednesday => "Wed",
                    DayOfWeek.Thursday => "Thu",
                    DayOfWeek.Friday => "Fri",
                    DayOfWeek.Saturday => "Sat",
                    _ => "Sun"
                },
                Orders = orderDays.Count(x => x == day),
                Shipments = shipmentDays.Count(x => x == day)
            })
            .ToList();

        var purchaseOrders = await _db.Orders
            .AsNoTracking()
            .Where(x => x.IsActive && x.OrderType == OrderType.Purchase && x.OrderDateUtc >= fromDate && x.OrderDateUtc < toDate)
            .Where(x => !hasWarehouseFilter || (x.WarehouseId.HasValue && warehouseIds.Contains(x.WarehouseId.Value)))
            .Select(x => new { x.RequiredDateUtc, x.FulfilledDateUtc })
            .ToListAsync(cancellationToken);

        var qualityChecks = await _db.ProductionQualityChecks
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.CheckDate >= fromDate && x.CheckDate < toDate)
            .Join(
                _db.ProductionOrders.AsNoTracking().Where(x => !x.IsDeleted),
                qc => qc.ProductionOrderId,
                po => po.Id,
                (qc, po) => new { qc, po })
            .Where(x => !hasWarehouseFilter || warehouseIds.Contains(x.po.WarehouseId))
            .Select(x => x.qc.Status)
            .ToListAsync(cancellationToken);

        var productionExecs = await _db.ProductionExecutions
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.CreatedAtUtc >= fromDate && x.CreatedAtUtc < toDate)
            .Join(
                _db.WorkCenters.AsNoTracking().Where(x => !x.IsDeleted),
                pe => pe.WorkCenterId,
                wc => wc.Id,
                (pe, wc) => new { pe, wc })
            .Where(x => !hasWarehouseFilter || warehouseIds.Contains(x.wc.WarehouseId))
            .Select(x => new { x.pe.PlannedQuantity, x.pe.CompletedQuantity })
            .ToListAsync(cancellationToken);

        var allPaymentsQuery = _db.Payments
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.PaymentDate >= fromDate && x.PaymentDate < toDate);

        var allPayments = await allPaymentsQuery.CountAsync(cancellationToken);
        var paidPayments = await allPaymentsQuery.CountAsync(x => x.Status == PaymentStatus.Paid, cancellationToken);

        var delayedShipmentCount = await _db.ShipmentExceptions
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.ReportedAtUtc >= fromDate && x.ReportedAtUtc < toDate)
            .Where(x => x.ExceptionType == ShipmentExceptionType.Delay)
            .Join(
                _db.Shipments.AsNoTracking().Where(x => !x.IsDeleted),
                ex => ex.ShipmentId,
                shipment => shipment.Id,
                (ex, shipment) => new { ex, shipment })
            .Where(x => !hasWarehouseFilter || warehouseIds.Contains(x.shipment.WarehouseId))
            .CountAsync(cancellationToken);

        var shipmentItems = await _db.ShipmentItems
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.CreatedAtUtc >= fromDate && x.CreatedAtUtc < toDate)
            .Where(x => !hasWarehouseFilter || warehouseIds.Contains(x.WarehouseId))
            .Join(
                _db.Products.AsNoTracking().Where(x => !x.IsDeleted),
                si => si.ProductId,
                p => p.Id,
                (si, p) => new { si, p })
            .Join(
                _db.Categories.AsNoTracking().Where(x => !x.IsDeleted),
                sp => sp.p.CategoryId,
                c => c.Id,
                (sp, c) => c.Name)
            .ToListAsync(cancellationToken);

        var shipmentInventoryMix = shipmentItems
            .GroupBy(x => x)
            .Select(g => new PieSliceReadModel
            {
                Label = g.Key,
                Value = g.Count()
            })
            .OrderByDescending(x => x.Value)
            .Take(4)
            .ToList();

        var procurementMeasured = purchaseOrders.Count(x => x.RequiredDateUtc.HasValue && x.FulfilledDateUtc.HasValue);
        var procurementSla = procurementMeasured == 0
            ? 0m
            : (decimal)purchaseOrders.Count(x =>
                x.RequiredDateUtc.HasValue &&
                x.FulfilledDateUtc.HasValue &&
                x.FulfilledDateUtc <= x.RequiredDateUtc) / procurementMeasured * 100m;

        var warehouseSummary = await GetWarehouseSummaryAsync(fromDate, toDate, warehouseIds, cancellationToken);

        var productionEfficiency = productionExecs.Sum(x => x.PlannedQuantity) <= 0m
            ? 0m
            : productionExecs.Sum(x => x.CompletedQuantity) / productionExecs.Sum(x => x.PlannedQuantity) * 100m;

        var financeCompletion = allPayments == 0 ? 0m : (decimal)paidPayments / allPayments * 100m;

        var operationsCompletion = qualityChecks.Count == 0
            ? 0m
            : (decimal)qualityChecks.Count(x => x == QualityCheckStatus.Passed) / qualityChecks.Count * 100m;

        var lowStockItems = await GetLowStockItemsAsync(warehouseIds, cancellationToken);
        var topLowStock = lowStockItems.OrderBy(x => x.Stock).FirstOrDefault();

        var insight = new DashboardInsightReadModel
        {
            Title = "AI Insight",
            Message = topLowStock != null
                ? $"{topLowStock.Item} in {topLowStock.Warehouse} is at {topLowStock.Stock:0.#} units against reorder level {topLowStock.ReorderLevel:0.#}."
                : "No critical shortages detected in the selected range."
        };

        return new DashboardOperationsReadModel
        {
            WeeklyOrdersVsShipments = weeklyOrdersVsShipments,
            TeamTaskCompletion =
            [
                new DashboardProgressMetricReadModel { Label = "Procurement", Value = Math.Round(procurementSla, 1) },
                new DashboardProgressMetricReadModel { Label = "Warehouse", Value = warehouseSummary.AveragePickAccuracy },
                new DashboardProgressMetricReadModel { Label = "Production", Value = Math.Round(productionEfficiency, 1) },
                new DashboardProgressMetricReadModel { Label = "Finance", Value = Math.Round(financeCompletion, 1) },
                new DashboardProgressMetricReadModel { Label = "Operations", Value = Math.Round(operationsCompletion, 1) }
            ],
            ShipmentInventoryMix = shipmentInventoryMix,
            Insight = insight,
            DelayedShipmentCount = delayedShipmentCount
        };
    }

    private async Task<List<WorkflowPipelineReadModel>> GetWorkflowPipelineAsync(
        DateTime fromDate,
        DateTime toDate,
        List<Guid> warehouseIds,
        CancellationToken cancellationToken)
    {
        var orderQuery = _db.Orders
            .AsNoTracking()
            .Where(x => x.IsActive && x.OrderDateUtc >= fromDate && x.OrderDateUtc < toDate);

        if (HasWarehouseFilter(warehouseIds))
        {
            orderQuery = orderQuery.Where(x =>
                x.WarehouseId.HasValue &&
                warehouseIds.Contains(x.WarehouseId.Value));
        }

        var productionQuery = _db.ProductionOrders
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.CreatedAtUtc >= fromDate && x.CreatedAtUtc < toDate);

        if (HasWarehouseFilter(warehouseIds))
        {
            productionQuery = productionQuery.Where(x => warehouseIds.Contains(x.WarehouseId));
        }

        var shipmentQuery = _db.Shipments
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.CreatedAtUtc >= fromDate && x.CreatedAtUtc < toDate);

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
        DateTime toDate,
        List<Guid> warehouseIds,
        CancellationToken cancellationToken)
    {
        var orderQuery = _db.Orders
            .AsNoTracking()
            .Where(x => x.IsActive && x.CreatedAtUtc >= fromDate && x.CreatedAtUtc < toDate);

        if (HasWarehouseFilter(warehouseIds))
        {
            orderQuery = orderQuery.Where(x =>
                x.WarehouseId.HasValue &&
                warehouseIds.Contains(x.WarehouseId.Value));
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

        var shipmentQuery = _db.Shipments
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.CreatedAtUtc >= fromDate && x.CreatedAtUtc < toDate);

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

        var productionQuery = _db.ProductionOrders
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.CreatedAtUtc >= fromDate && x.CreatedAtUtc < toDate);

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

    private async Task<WorkforceSummaryReadModel> GetWorkforceSummaryAsync(
        CancellationToken cancellationToken)
    {
        var activeStaff = await _db.Users
            .AsNoTracking()
            .CountAsync(x => x.IsActive && !x.IsDeleted, cancellationToken);

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
        DateTime toDate,
        List<Guid> warehouseIds,
        CancellationToken cancellationToken)
    {
        var purchaseOrdersQuery = _db.Orders
            .AsNoTracking()
            .Where(x => x.IsActive && x.OrderType == OrderType.Purchase && x.OrderDateUtc >= fromDate && x.OrderDateUtc < toDate);

        if (HasWarehouseFilter(warehouseIds))
        {
            purchaseOrdersQuery = purchaseOrdersQuery.Where(x =>
                x.WarehouseId.HasValue &&
                warehouseIds.Contains(x.WarehouseId.Value));
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
            : (decimal)purchaseOrders.Count(x =>
                x.RequiredDateUtc.HasValue &&
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
        DateTime toDate,
        List<Guid> warehouseIds,
        CancellationToken cancellationToken)
    {
        var warehouseQuery = _db.Warehouses
            .AsNoTracking()
            .Where(x => x.IsActive && !x.IsDeleted);

        if (HasWarehouseFilter(warehouseIds))
        {
            warehouseQuery = warehouseQuery.Where(x => warehouseIds.Contains(x.Id));
        }

        var warehousesActive = await warehouseQuery.CountAsync(cancellationToken);

        var shipmentQuery = _db.Shipments
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.CreatedAtUtc >= fromDate && x.CreatedAtUtc < toDate);

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
        DateTime toDate,
        List<Guid> warehouseIds,
        CancellationToken cancellationToken)
    {
        var ordersQuery = _db.Orders
            .AsNoTracking()
            .Include(x => x.Warehouse)
            .Where(x => x.IsActive && x.OrderDateUtc >= fromDate && x.OrderDateUtc < toDate);

        if (HasWarehouseFilter(warehouseIds))
        {
            ordersQuery = ordersQuery.Where(x =>
                x.WarehouseId.HasValue &&
                warehouseIds.Contains(x.WarehouseId.Value));
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
                Status = x.Stock.QuantityAvailable <= 0m
                    ? "Critical"
                    : x.Stock.QuantityAvailable <= x.Product.ReorderLevel * 0.5m
                        ? "Critical"
                        : "Low"
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