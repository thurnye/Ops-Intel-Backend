using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

public class DashboardService : IDashboardService
{
    private readonly IDashboardReadRepository _dashboardReadRepository;

    public DashboardService(IDashboardReadRepository dashboardReadRepository)
    {
        _dashboardReadRepository = dashboardReadRepository;
    }

    public async Task<DashboardOverviewViewResponse> GetOverviewAsync(
        DashboardFilterRequest request,
        CancellationToken cancellationToken = default)
    {
        var readModel = await _dashboardReadRepository.GetOverviewAsync(
            request.Range?.From,
            request.Range?.To,
            request.Site,
            cancellationToken);

        return MapToViewResponse(readModel);
    }

    private static DashboardOverviewViewResponse MapToViewResponse(DashboardOverviewReadModel source)
    {
        return new DashboardOverviewViewResponse
        {
            Header = BuildHeader(),
            AnalyticsHeader = new DashboardSectionHeaderDto
            {
                Title = DashboardTitles.OperationalAnalytics,
                Subtitle = "Visual insights across inventory, production, shipment, and finance."
            },

            Kpis = BuildKpis(source),
            BusinessPerformance = BuildBusinessPerformance(source),
            AttentionRequired = BuildAttentionRequired(source),

            Finance = BuildFinance(source),
            ModuleHealth = BuildModuleHealth(source),
            Inventory = BuildInventory(source),
            Production = BuildProduction(source),
            Shipments = BuildShipments(source),

            SummarySnapshots = BuildSummarySnapshots(source),
            Workflow = BuildWorkflow(source),
            ActivityFeed = BuildActivityFeed(source),
            Tables = BuildTables(source)
        };
    }

    private static DashboardHeaderDto BuildHeader()
    {
        return new DashboardHeaderDto
        {
            Title = DashboardTitles.Dashboard,
            Subtitle = "Monitor operations, finances, inventory, production, and shipment performance in one place.",
            RangeOptions = [],
            SiteOptions =
            [
                new DashboardOptionDto { Value = "all", Label = DashboardLabels.AllSites },
                new DashboardOptionDto { Value = "toronto", Label = DashboardLabels.Toronto },
                new DashboardOptionDto { Value = "vaughan", Label = DashboardLabels.Vaughan },
                new DashboardOptionDto { Value = "hamilton", Label = DashboardLabels.Hamilton }
            ],
            RefreshLabel = DashboardLabels.Refresh,
            ExportLabel = DashboardLabels.Export
        };
    }

    private static List<DashboardKpiCardDto> BuildKpis(DashboardOverviewReadModel source)
    {
        return
        [
            new DashboardKpiCardDto
            {
                Id = "revenue",
                Title = DashboardTitles.TotalRevenue,
                Value = FormatCurrencyCompact(source.Kpis.TotalRevenue),
                Change = FormatSignedPercent(source.KpiComparison.RevenueChangePercent),
                Subtext = "vs previous period",
                IconKey = DashboardIcons.Currency,
                Color = DashboardColors.PrimaryBlue,
                Direction = source.KpiComparison.RevenueChangePercent >= 0 ? "up" : "down"
            },
            new DashboardKpiCardDto
            {
                Id = "orders",
                Title = DashboardTitles.OrdersInProgress,
                Value = source.Kpis.OrdersInProgress.ToString("N0"),
                Change = FormatSignedPercent(source.KpiComparison.OrdersInProgressChangePercent),
                Subtext = $"{source.BusinessPerformance.ApprovalQueue} awaiting approval",
                IconKey = DashboardIcons.Orders,
                Color = DashboardColors.Purple,
                Direction = source.KpiComparison.OrdersInProgressChangePercent >= 0 ? "up" : "down"
            },
            new DashboardKpiCardDto
            {
                Id = "production",
                Title = DashboardTitles.ProductionEfficiency,
                Value = $"{source.Kpis.ProductionEfficiency:0.0}%",
                Change = FormatSignedPercent(source.KpiComparison.ProductionEfficiencyChangePercent),
                Subtext = "vs previous period",
                IconKey = DashboardIcons.Production,
                Color = DashboardColors.Orange,
                Direction = source.KpiComparison.ProductionEfficiencyChangePercent >= 0 ? "up" : "down"
            },
            new DashboardKpiCardDto
            {
                Id = "inventory",
                Title = DashboardTitles.InventoryValue,
                Value = FormatCurrencyCompact(source.Kpis.InventoryValue),
                Change = FormatSignedPercent(source.KpiComparison.InventoryValueChangePercent),
                Subtext = $"{source.LowStockItems.Count} low-stock SKUs",
                IconKey = DashboardIcons.Inventory,
                Color = DashboardColors.Teal,
                Direction = source.KpiComparison.InventoryValueChangePercent >= 0 ? "up" : "down"
            },
            new DashboardKpiCardDto
            {
                Id = "shipments",
                Title = DashboardTitles.ShipmentsPending,
                Value = source.Kpis.ShipmentsPending.ToString("N0"),
                Change = FormatSignedPercent(source.KpiComparison.ShipmentsPendingChangePercent),
                Subtext = $"{source.Operations.DelayedShipmentCount} delayed",
                IconKey = DashboardIcons.Shipments,
                Color = DashboardColors.Red,
                Direction = source.KpiComparison.ShipmentsPendingChangePercent >= 0 ? "up" : "down"
            },
            new DashboardKpiCardDto
            {
                Id = "alerts",
                Title = DashboardTitles.CriticalAlerts,
                Value = source.Kpis.CriticalAlerts.ToString("N0"),
                Change = FormatSignedPercent(source.KpiComparison.CriticalAlertsChangePercent),
                Subtext = source.Kpis.CriticalAlerts > 0 ? "needs review" : "stable",
                IconKey = DashboardIcons.Alerts,
                Color = DashboardColors.Amber,
                Direction = source.KpiComparison.CriticalAlertsChangePercent >= 0 ? "up" : "down"
            }
        ];
    }

    private static DashboardBusinessPerformanceSectionDto BuildBusinessPerformance(DashboardOverviewReadModel source)
    {
        return new DashboardBusinessPerformanceSectionDto
        {
            Title = DashboardSections.BusinessPerformance,
            MetricChips = ["Revenue", "Orders", "Shipments"],
            RevenueTrendTitle = DashboardTitles.RevenueTrend,
            RevenueTrend = new DashboardLineChartDto
            {
                Labels = source.BusinessPerformance.MonthlyRevenueTrend.Select(x => x.Label).ToList(),
                Series =
                [
                    new DashboardChartSeriesDto
                    {
                        Label = "Revenue (CAD '000)",
                        Data = source.BusinessPerformance.MonthlyRevenueTrend.Select(x => x.Value).ToList()
                    }
                ]
            },
            ProgressCards =
            [
                new DashboardProgressCardDto
                {
                    Id = "on-time",
                    Title = "On-Time Shipment Rate",
                    Value = $"{source.BusinessPerformance.OnTimeShipmentRate:0.0}%",
                    Progress = source.BusinessPerformance.OnTimeShipmentRate,
                    Color = "primary"
                },
                new DashboardProgressCardDto
                {
                    Id = "capacity",
                    Title = "Warehouse Capacity Use",
                    Value = $"{source.BusinessPerformance.WarehouseCapacityUse:0.#}%",
                    Progress = source.BusinessPerformance.WarehouseCapacityUse,
                    Color = "warning"
                },
                new DashboardProgressCardDto
                {
                    Id = "approvals",
                    Title = "Approval Queue",
                    Value = source.BusinessPerformance.ApprovalQueue.ToString(),
                    Progress = source.BusinessPerformance.ApprovalQueue,
                    Description = "Pending commercial / finance approval queue.",
                    Color = "info"
                }
            ]
        };
    }

    private static DashboardAttentionRequiredSectionDto BuildAttentionRequired(DashboardOverviewReadModel source)
    {
        return new DashboardAttentionRequiredSectionDto
        {
            Title = DashboardSections.AttentionRequired,
            Alerts = source.Alerts.Select(MapAlert).ToList(),
            QuickActionsTitle = DashboardLabels.QuickActions,
            QuickActions =
            [
                new DashboardQuickActionDto { Id = "create-order", Label = "Create Order", IconKey = DashboardIcons.Task, Variant = "contained" },
                new DashboardQuickActionDto { Id = "new-po", Label = "New PO", IconKey = DashboardIcons.Shopping, Variant = "outlined" },
                new DashboardQuickActionDto { Id = "add-inventory", Label = "Add Inventory", IconKey = DashboardIcons.Warehouse, Variant = "outlined" },
                new DashboardQuickActionDto { Id = "schedule-shipment", Label = "Schedule Shipment", IconKey = DashboardIcons.Shipments, Variant = "outlined" }
            ]
        };
    }

    private static DashboardFinanceSectionDto BuildFinance(DashboardOverviewReadModel source)
    {
        return new DashboardFinanceSectionDto
        {
            SectionTitle = DashboardTitles.FinanceAnalytics,
            RevenueExpenseTitle = DashboardTitles.RevenueVsExpense,
            ExpenseBreakdownTitle = DashboardTitles.ExpenseBreakdown,
            RevenueExpenseTrend = new DashboardLineChartDto
            {
                Labels = source.Finance.RevenueExpenseTrend.Select(x => x.Label).ToList(),
                Series =
                [
                    new DashboardChartSeriesDto
                    {
                        Label = "Revenue",
                        Data = source.Finance.RevenueExpenseTrend.Select(x => x.Series1).ToList()
                    },
                    new DashboardChartSeriesDto
                    {
                        Label = "Expenses",
                        Data = source.Finance.RevenueExpenseTrend.Select(x => x.Series2).ToList()
                    },
                    new DashboardChartSeriesDto
                    {
                        Label = "Net Cashflow",
                        Data = source.Finance.RevenueExpenseTrend.Select(x => x.Series3 ?? 0m).ToList()
                    }
                ]
            },
            ExpenseBreakdown = source.Finance.ExpenseBreakdown
                .Select((x, index) => new DashboardPieSliceDto
                {
                    Id = index,
                    Label = x.Label,
                    Value = x.Value
                })
                .ToList()
        };
    }

    private static DashboardModuleHealthSectionDto BuildModuleHealth(DashboardOverviewReadModel source)
    {
        return new DashboardModuleHealthSectionDto
        {
            Cards = source.ModuleHealth.Select(x => new DashboardModuleHealthCardDto
            {
                Id = x.Module.ToLowerInvariant(),
                Title = x.Module,
                Value = x.Module == "Finance"
                    ? FormatCurrencyCompact(TryParseDecimal(x.Value))
                    : x.Value,
                Status = x.Status,
                Note = x.Note,
                IconKey = x.Module switch
                {
                    "Inventory" => DashboardIcons.Inventory,
                    "Production" => DashboardIcons.Production,
                    "Shipment" => DashboardIcons.Shipments,
                    "Finance" => DashboardIcons.Check,
                    _ => DashboardIcons.Task
                },
                Color = x.Module switch
                {
                    "Inventory" => DashboardColors.Teal,
                    "Production" => DashboardColors.Orange,
                    "Shipment" => DashboardColors.Red,
                    "Finance" => DashboardColors.PrimaryBlue,
                    _ => DashboardColors.PrimaryBlue
                }
            }).ToList()
        };
    }

    private static DashboardInventorySectionDto BuildInventory(DashboardOverviewReadModel source)
    {
        return new DashboardInventorySectionDto
        {
            SectionTitle = DashboardTitles.InventoryAnalytics,
            LowStockTitle = DashboardTitles.LowStockItems,
            LowStockChart = new DashboardBarChartDto
            {
                Labels = source.Inventory.TopLowStockItems.Select(x => x.Label).ToList(),
                Layout = "horizontal",
                Series =
                [
                    new DashboardChartSeriesDto
                    {
                        Label = "Available Units",
                        Data = source.Inventory.TopLowStockItems.Select(x => x.Value).ToList()
                    }
                ]
            },
            InflowOutflowTitle = DashboardTitles.InventoryInflowOutflow,
            InflowOutflowChart = new DashboardLineChartDto
            {
                Labels = source.Inventory.InventoryInflowOutflow.Select(x => x.Label).ToList(),
                Series =
                [
                    new DashboardChartSeriesDto
                    {
                        Label = "Inflow",
                        Data = source.Inventory.InventoryInflowOutflow.Select(x => x.Series1).ToList()
                    },
                    new DashboardChartSeriesDto
                    {
                        Label = "Outflow",
                        Data = source.Inventory.InventoryInflowOutflow.Select(x => x.Series2).ToList()
                    }
                ]
            },
            WarehouseCompositionTitle = DashboardTitles.WarehouseComposition,
            WarehouseCompositionChart = new DashboardBarChartDto
            {
                Labels = source.Inventory.WarehouseStockComposition.Select(x => x.Warehouse).ToList(),
                Series =
                [
                    new DashboardChartSeriesDto
                    {
                        Label = "Raw Materials",
                        Data = source.Inventory.WarehouseStockComposition.Select(x => x.RawMaterials).ToList(),
                        Stack = "stock"
                    },
                    new DashboardChartSeriesDto
                    {
                        Label = "Finished Goods",
                        Data = source.Inventory.WarehouseStockComposition.Select(x => x.FinishedGoods).ToList(),
                        Stack = "stock"
                    },
                    new DashboardChartSeriesDto
                    {
                        Label = "Packaging",
                        Data = source.Inventory.WarehouseStockComposition.Select(x => x.Packaging).ToList(),
                        Stack = "stock"
                    }
                ]
            },
            InventoryMixTitle = DashboardTitles.InventoryMix,
            InventoryMix = source.Inventory.InventoryMixByCategory
                .Select((x, index) => new DashboardPieSliceDto
                {
                    Id = index,
                    Label = x.Label,
                    Value = x.Value
                })
                .ToList()
        };
    }

    private static DashboardProductionSectionDto BuildProduction(DashboardOverviewReadModel source)
    {
        var labels = source.Production.ProductionLineEfficiency
            .FirstOrDefault()?.Points.Select(x => x.Label).ToList() ?? [];

        return new DashboardProductionSectionDto
        {
            SectionTitle = DashboardTitles.ProductionAnalytics,
            EfficiencyTitle = DashboardTitles.ProductionEfficiencyComparison,
            EfficiencyChart = new DashboardLineChartDto
            {
                Labels = labels,
                Series = source.Production.ProductionLineEfficiency
                    .Select(x => new DashboardChartSeriesDto
                    {
                        Label = x.Line,
                        Data = x.Points.Select(p => p.Value).ToList()
                    })
                    .ToList()
            },
            StatusMixTitle = "Production Job Status Mix",
            StatusMix = source.Production.ProductionJobStatusMix
                .Select((x, index) => new DashboardPieSliceDto
                {
                    Id = index,
                    Label = x.Label,
                    Value = x.Value
                })
                .ToList(),
            PlannedVsActualTitle = DashboardTitles.PlannedVsActual,
            PlannedVsActualChart = new DashboardBarChartDto
            {
                Labels = source.Production.PlannedVsActualOutput.Select(x => x.Label).ToList(),
                Series =
                [
                    new DashboardChartSeriesDto
                    {
                        Label = "Planned Output",
                        Data = source.Production.PlannedVsActualOutput.Select(x => x.Series1).ToList()
                    },
                    new DashboardChartSeriesDto
                    {
                        Label = "Actual Output",
                        Data = source.Production.PlannedVsActualOutput.Select(x => x.Series2).ToList()
                    }
                ]
            }
        };
    }

    private static DashboardShipmentSectionDto BuildShipments(DashboardOverviewReadModel source)
    {
        return new DashboardShipmentSectionDto
        {
            SectionTitle = DashboardTitles.ShipmentAnalytics,
            OnTimeVsDelayedTitle = DashboardTitles.ShipmentTrend,
            OnTimeVsDelayedChart = new DashboardLineChartDto
            {
                Labels = source.Shipment.OnTimeVsDelayedTrend.Select(x => x.Label).ToList(),
                Series =
                [
                    new DashboardChartSeriesDto
                    {
                        Label = "On-Time %",
                        Data = source.Shipment.OnTimeVsDelayedTrend.Select(x => x.Series1).ToList()
                    },
                    new DashboardChartSeriesDto
                    {
                        Label = "Delayed %",
                        Data = source.Shipment.OnTimeVsDelayedTrend.Select(x => x.Series2).ToList()
                    }
                ]
            },
            StatusDistributionTitle = "Shipment Status Distribution",
            StatusDistribution = source.Shipment.ShipmentStatusDistribution
                .Select((x, index) => new DashboardPieSliceDto
                {
                    Id = index,
                    Label = x.Label,
                    Value = x.Value
                })
                .ToList(),
            WeeklyOrdersVsShipmentsTitle = "Weekly Orders vs Shipments",
            WeeklyOrdersVsShipmentsChart = new DashboardBarChartDto
            {
                Labels = source.Operations.WeeklyOrdersVsShipments.Select(x => x.Label).ToList(),
                Series =
                [
                    new DashboardChartSeriesDto
                    {
                        Label = "Orders",
                        Data = source.Operations.WeeklyOrdersVsShipments.Select(x => x.Orders).ToList()
                    },
                    new DashboardChartSeriesDto
                    {
                        Label = "Shipments",
                        Data = source.Operations.WeeklyOrdersVsShipments.Select(x => x.Shipments).ToList()
                    }
                ]
            },
            TeamTaskCompletionTitle = "Team Task Completion",
            TeamTaskCompletion = source.Operations.TeamTaskCompletion
                .Select(x => new DashboardProgressStatDto
                {
                    Label = x.Label,
                    Value = x.Value
                })
                .ToList(),
            InventoryMixTitle = "Inventory Mix",
            InventoryMix = source.Operations.ShipmentInventoryMix
                .Select((x, index) => new DashboardPieSliceDto
                {
                    Id = index,
                    Label = x.Label,
                    Value = x.Value
                })
                .ToList()
        };
    }

    private static List<DashboardSummarySnapshotDto> BuildSummarySnapshots(DashboardOverviewReadModel source)
    {
        return
        [
            new DashboardSummarySnapshotDto
            {
                Id = "workforce",
                Title = DashboardSections.Workforce,
                IconKey = DashboardIcons.Groups,
                AccentTone = "info",
                PrimaryLabel = "Active Staff",
                PrimaryValue = source.WorkforceSummary.ActiveStaff.ToString("N0"),
                Stats =
                [
                    new DashboardLabelValueDto { Label = "Shift coverage", Value = $"{source.WorkforceSummary.ShiftCoverage:0.#}%" },
                    new DashboardLabelValueDto { Label = "Open positions", Value = source.WorkforceSummary.OpenPositions.ToString() }
                ]
            },
            new DashboardSummarySnapshotDto
            {
                Id = "procurement",
                Title = DashboardSections.Procurement,
                IconKey = DashboardIcons.Shopping,
                AccentTone = "warning",
                PrimaryLabel = "Open Purchase Orders",
                PrimaryValue = source.ProcurementSummary.OpenPurchaseOrders.ToString("N0"),
                Stats =
                [
                    new DashboardLabelValueDto { Label = "Awaiting approval", Value = source.ProcurementSummary.AwaitingApproval.ToString() },
                    new DashboardLabelValueDto { Label = "Supplier SLA met", Value = $"{source.ProcurementSummary.SupplierSlaMet:0.#}%" }
                ]
            },
            new DashboardSummarySnapshotDto
            {
                Id = "warehouse",
                Title = DashboardSections.Warehouse,
                IconKey = DashboardIcons.Warehouse,
                AccentTone = "success",
                PrimaryLabel = "Warehouses Active",
                PrimaryValue = source.WarehouseSummary.WarehousesActive.ToString("N0"),
                Stats =
                [
                    new DashboardLabelValueDto { Label = "Avg. pick accuracy", Value = $"{source.WarehouseSummary.AveragePickAccuracy:0.#}%" },
                    new DashboardLabelValueDto { Label = "Cross-dock utilization", Value = $"{source.WarehouseSummary.CrossDockUtilization:0.#}%" }
                ]
            }
        ];
    }

    private static DashboardWorkflowSectionDto BuildWorkflow(DashboardOverviewReadModel source)
    {
        return new DashboardWorkflowSectionDto
        {
            Title = DashboardSections.Workflow,
            Steps = source.WorkflowPipeline.Select((x, index) => new DashboardWorkflowStepDto
            {
                Label = x.Label,
                Count = x.Count,
                Progress = x.Progress,
                Color = index switch
                {
                    0 => DashboardColors.PrimaryBlue,
                    1 => DashboardColors.Purple,
                    2 => DashboardColors.Orange,
                    3 => DashboardColors.Teal,
                    4 => DashboardColors.Green,
                    5 => DashboardColors.Red,
                    _ => DashboardColors.Indigo
                }
            }).ToList()
        };
    }

    private static DashboardActivityFeedSectionDto BuildActivityFeed(DashboardOverviewReadModel source)
    {
        return new DashboardActivityFeedSectionDto
        {
            Title = DashboardSections.ActivityFeed,
            Items = source.ActivityFeed.Select((x, index) => new DashboardActivityItemDto
            {
                Id = index + 1,
                Text = x.Text,
                Time = ToRelativeTime(x.TimeUtc),
                Color = x.Type switch
                {
                    "Order" => DashboardColors.Green,
                    "Shipment" => DashboardColors.PrimaryBlue,
                    "Production" => DashboardColors.Purple,
                    _ => DashboardColors.Teal
                }
            }).ToList(),
            Insight = new DashboardInsightDto
            {
                Title = source.Operations.Insight.Title,
                Message = source.Operations.Insight.Message,
                IconKey = DashboardIcons.Task
            }
        };
    }

    private static DashboardTablesSectionDto BuildTables(DashboardOverviewReadModel source)
    {
        return new DashboardTablesSectionDto
        {
            RecentOrdersTitle = "Recent Orders",
            LowStockItemsTitle = "Low Stock Items",
            RecentOrders = source.RecentOrders.Select(MapRecentOrder).ToList(),
            LowStockItems = source.LowStockItems.Select(MapLowStockItem).ToList()
        };
    }

    private static string FormatCurrencyCompact(decimal value)
    {
        if (value >= 1_000_000m)
            return $"${value / 1_000_000m:0.##}M";

        if (value >= 1_000m)
            return $"${value / 1_000m:0.##}K";

        return $"${value:0.##}";
    }

    private static string FormatSignedPercent(decimal value)
    {
        var rounded = Math.Round(value, 1);
        return rounded >= 0m ? $"+{rounded:0.0}%" : $"{rounded:0.0}%";
    }

    private static decimal TryParseDecimal(string value)
    {
        return decimal.TryParse(value, out var parsed) ? parsed : 0m;
    }

    private static string ToRelativeTime(DateTime utcTime)
    {
        var diff = DateTime.UtcNow - utcTime;

        if (diff.TotalMinutes < 1) return "just now";
        if (diff.TotalMinutes < 60) return $"{Math.Floor(diff.TotalMinutes)} min ago";
        if (diff.TotalHours < 24) return $"{Math.Floor(diff.TotalHours)} hr ago";
        return $"{Math.Floor(diff.TotalDays)} day ago";
    }

    private static DashboardAlertDto MapAlert(DashboardAlertReadModel source)
    {
        return new DashboardAlertDto
        {
            Title = source.Title,
            Detail = source.Detail,
            Severity = source.Severity
        };
    }

    private static RecentOrderDto MapRecentOrder(RecentOrderReadModel source)
    {
        return new RecentOrderDto
        {
            OrderNo = source.OrderNo,
            Customer = source.Customer,
            Module = source.Module,
            Amount = source.Amount,
            Status = source.Status,
            DueDate = source.DueDate,
            Warehouse = source.Warehouse
        };
    }

    private static LowStockItemDto MapLowStockItem(LowStockItemReadModel source)
    {
        return new LowStockItemDto
        {
            Sku = source.Sku,
            Item = source.Item,
            Warehouse = source.Warehouse,
            Stock = source.Stock,
            ReorderLevel = source.ReorderLevel,
            Status = source.Status
        };
    }
}