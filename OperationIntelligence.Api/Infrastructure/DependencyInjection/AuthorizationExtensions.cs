
using OperationIntelligence.Core;

namespace OperationIntelligence.Api
{
    public static class AuthorizationExtensions
    {
        public static IServiceCollection AddAppAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                // ========================
                // USER MANAGEMENT
                // ========================

                options.AddPolicy("AuthUsersRead", policy =>
                    policy.RequireRole(UserRoleNames.SuperAdmin, UserRoleNames.Admin)
                          .RequireClaim("permission", PermissionCodes.AuthUsersRead));

                options.AddPolicy("AuthUsersWrite", policy =>
                    policy.RequireRole(UserRoleNames.SuperAdmin, UserRoleNames.Admin)
                          .RequireClaim("permission", PermissionCodes.AuthUsersWrite));

                // ========================
                // ROLE MANAGEMENT
                // ========================

                options.AddPolicy("AuthRolesRead", policy =>
                    policy.RequireRole(UserRoleNames.SuperAdmin, UserRoleNames.Admin)
                          .RequireClaim("permission", PermissionCodes.AuthRolesRead));

                options.AddPolicy("AuthRolesWrite", policy =>
                    policy.RequireRole(UserRoleNames.SuperAdmin)
                          .RequireClaim("permission", PermissionCodes.AuthRolesWrite));

                // ========================
                // PERMISSION MANAGEMENT
                // ========================

                options.AddPolicy("AuthPermissionsRead", policy =>
                    policy.RequireRole(UserRoleNames.SuperAdmin, UserRoleNames.Admin)
                          .RequireClaim("permission", PermissionCodes.AuthPermissionsRead));

                options.AddPolicy("AuthPermissionsWrite", policy =>
                    policy.RequireRole(UserRoleNames.SuperAdmin)
                          .RequireClaim("permission", PermissionCodes.AuthPermissionsWrite));

                // ========================
                // SESSION MANAGEMENT
                // ========================

                options.AddPolicy("AuthSessionsRead", policy =>
                    policy.RequireRole(UserRoleNames.SuperAdmin, UserRoleNames.Admin)
                          .RequireClaim("permission", PermissionCodes.AuthSessionsRead));

                options.AddPolicy("AuthSessionsRevoke", policy =>
                    policy.RequireRole(UserRoleNames.SuperAdmin, UserRoleNames.Admin)
                          .RequireClaim("permission", PermissionCodes.AuthSessionsRevoke));

                // ========================
                // PRODUCTION OPERATIONS
                // ========================

                options.AddPolicy("ProductionManage", policy =>
                    policy.RequireRole(
                        UserRoleNames.SuperAdmin,
                        UserRoleNames.Admin,
                        UserRoleNames.ProductionManager));

                options.AddPolicy("ProductionOperate", policy =>
                    policy.RequireRole(
                        UserRoleNames.SuperAdmin,
                        UserRoleNames.Admin,
                        UserRoleNames.ProductionManager,
                        UserRoleNames.Operator));

                // ========================
                // SCHEDULING
                // ========================

                options.AddPolicy("SchedulingManage", policy =>
                    policy.RequireRole(
                        UserRoleNames.SuperAdmin,
                        UserRoleNames.Admin,
                        UserRoleNames.Planner));

                // ========================
                // INVENTORY / WAREHOUSE
                // ========================

                options.AddPolicy("InventoryManage", policy =>
                    policy.RequireRole(
                        UserRoleNames.SuperAdmin,
                        UserRoleNames.Admin,
                        UserRoleNames.WarehouseManager));

                // ========================
                // READ-ONLY ACCESS
                // ========================

                options.AddPolicy("ReadOnlyAccess", policy =>
                    policy.RequireRole(
                        UserRoleNames.SuperAdmin,
                        UserRoleNames.Admin,
                        UserRoleNames.Viewer));
            });

            return services;
        }
    }
}