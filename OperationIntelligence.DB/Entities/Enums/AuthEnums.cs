namespace OperationIntelligence.DB;

    public enum UserRoles
    {
        SuperAdmin = 1,
        Admin = 2,
        Planner = 3,
        ProductionManager = 4,
        WarehouseManager = 5,
        Operator = 6,
        Viewer = 7
    }


public static class RoleNames
    {
        public const string SuperAdmin = "SUPER_ADMIN";
        public const string Admin = "ADMIN";
        public const string Planner = "PLANNER";
        public const string ProductionManager = "PRODUCTION_MANAGER";
        public const string WarehouseManager = "WAREHOUSE_MANAGER";
        public const string Operator = "OPERATOR";
        public const string Viewer = "VIEWER";
    }