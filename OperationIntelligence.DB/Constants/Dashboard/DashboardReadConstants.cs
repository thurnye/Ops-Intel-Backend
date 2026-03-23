namespace OperationIntelligence.DB;

public static class DashboardReadConstants
{
    public const string YearMonthLabelFormat = "{0}-{1:00}";

    public static class RangeKeys
    {
        public const string Last7Days = "7d";
        public const string Last30Days = "30d";
        public const string Last90Days = "90d";
        public const string Last12Months = "1y";
    }

    public static class SiteKeys
    {
        public const string All = "all";
    }

    public static class Severity
    {
        public const string Critical = "Critical";
        public const string Warning = "Warning";
        public const string Info = "Info";
        public const string Healthy = "Healthy";
        public const string Low = "Low";
    }

    public static class Modules
    {
        public const string Inventory = "Inventory";
        public const string Production = "Production";
        public const string Shipment = "Shipment";
        public const string Finance = "Finance";
    }

    public static class WorkflowLabels
    {
        public const string OrderBacklog = "Order backlog";
        public const string ProductionActive = "Production active";
        public const string ShipmentsActive = "Shipments active";
    }

    public static class ActivityTypes
    {
        public const string Order = "Order";
        public const string Shipment = "Shipment";
        public const string Production = "Production";
    }

    public static class TeamLabels
    {
        public const string Procurement = "Procurement";
        public const string Warehouse = "Warehouse";
        public const string Production = "Production";
        public const string Finance = "Finance";
        public const string Operations = "Operations";
    }

    public static class Insight
    {
        public const string Title = "AI Insight";
        public const string NoCriticalShortages = "No critical shortages detected in the selected range.";
        public const string ShortageMessageTemplate = "{0} in {1} is at {2:0.#} units against reorder level {3:0.#}.";
    }

    public static class CategoryMarkers
    {
        public const string Raw = "raw";
        public const string Finished = "finished";
        public const string Pack = "pack";
    }

    public static class Placeholders
    {
        public const string UnknownCustomer = "Unknown";
        public const string CustomerFallback = "customer";
        public const string NotAvailable = "N/A";
        public static readonly string[] WeekdayShortNames = ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"];
    }

    public static class Alerts
    {
        public const string LowStockTitleTemplate = "{0} items below reorder level";
        public const string LowStockDetail = "Inventory replenishment required across warehouses.";
        public const string DelayedShipmentsTitleTemplate = "{0} delayed shipment exceptions";
        public const string DelayedShipmentsDetail = "Shipment exceptions need review.";
        public const string PendingApprovalsTitleTemplate = "{0} orders awaiting approval";
        public const string PendingApprovalsDetail = "Commercial approval queue is building.";
        public const string ExecutionsBelowPlanTitleTemplate = "{0} executions below plan";
        public const string ExecutionsBelowPlanDetail = "Production throughput is below expected output.";
    }

    public static class Notes
    {
        public const string ItemsBelowReorderThreshold = "Items below reorder threshold";
        public const string ActiveJobsWithFailedQualityChecks = "Active jobs with failed quality checks detected";
        public const string ActiveJobsWithinQualityTargets = "Active jobs running within expected quality targets";
        public const string OpenShipmentExceptions = "Open shipment exceptions";
        public const string OutstandingReceivables = "Outstanding receivables in selected range";
    }

    public static class ActivityTemplates
    {
        public const string OrderCreated = "Order {0} created for {1}";
        public const string ShipmentStatus = "Shipment {0} is {1}";
        public const string ProductionStatus = "Production order {0} is {1}";
    }
}
