using OperationIntelligence.DB;

namespace OperationIntelligence.Core.Util
{
    public static class RoleExtensions
    {
        public static string ToRoleName(this UserRoles role)
        {
            return role switch
            {
                UserRoles.SuperAdmin => RoleNames.SuperAdmin,
                UserRoles.Admin => RoleNames.Admin,
                UserRoles.Planner => RoleNames.Planner,
                UserRoles.ProductionManager => RoleNames.ProductionManager,
                UserRoles.WarehouseManager => RoleNames.WarehouseManager,
                UserRoles.Operator => RoleNames.Operator,
                UserRoles.Viewer => RoleNames.Viewer,
                _ => throw new ArgumentOutOfRangeException(nameof(role), role, null)
            };
        }
    }
}