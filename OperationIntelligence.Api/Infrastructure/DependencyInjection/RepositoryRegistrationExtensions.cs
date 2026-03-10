using OperationIntelligence.DB;

namespace OperationIntelligence.Api
{
    public static class RepositoryRegistrationExtensions
    {
        public static IServiceCollection AddAppRepositories(this IServiceCollection services)
        {   
            // Auth
            services.AddScoped<IPlatformUserRepository, PlatformUserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IPermissionRepository, PermissionRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IUserSessionRepository, UserSessionRepository>();
            services.AddScoped<IEmailVerificationTokenRepository, EmailVerificationTokenRepository>();
            services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
            services.AddScoped<IPasswordHistoryRepository, PasswordHistoryRepository>();
            services.AddScoped<ILoginAttemptRepository, LoginAttemptRepository>();
            services.AddScoped<IMediaFileRepository, MediaFileRepository>();


            // Inventory
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IBrandRepository, BrandRepository>();
            services.AddScoped<IWarehouseRepository, WarehouseRepository>();
            services.AddScoped<ISupplierRepository, SupplierRepository>();
            services.AddScoped<IUnitOfMeasureRepository, UnitOfMeasureRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductImageRepository, ProductImageRepository>();
            services.AddScoped<IProductSupplierRepository, ProductSupplierRepository>();
            services.AddScoped<IInventoryStockRepository, InventoryStockRepository>();
            services.AddScoped<IStockMovementRepository, StockMovementRepository>();


            // Order
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderItemRepository, OrderItemRepository>();
            services.AddScoped<IOrderStatusHistoryRepository, OrderStatusHistoryRepository>();
            services.AddScoped<IOrderPaymentRepository, OrderPaymentRepository>();
            services.AddScoped<IOrderImageRepository, OrderImageRepository>();
            services.AddScoped<IOrderNoteRepository, OrderNoteRepository>();
            services.AddScoped<IOrderAddressRepository, OrderAddressRepository>();


            // Production
            services.AddScoped<IWorkCenterRepository, WorkCenterRepository>();
            services.AddScoped<IMachineRepository, MachineRepository>();
            services.AddScoped<IBillOfMaterialRepository, BillOfMaterialRepository>();
            services.AddScoped<IBillOfMaterialItemRepository, BillOfMaterialItemRepository>();
            services.AddScoped<IRoutingRepository, RoutingRepository>();
            services.AddScoped<IRoutingStepRepository, RoutingStepRepository>();
            services.AddScoped<IProductionOrderRepository, ProductionOrderRepository>();
            services.AddScoped<IProductionExecutionRepository, ProductionExecutionRepository>();
            services.AddScoped<IProductionMaterialIssueRepository, ProductionMaterialIssueRepository>();
            services.AddScoped<IProductionMaterialConsumptionRepository, ProductionMaterialConsumptionRepository>();
            services.AddScoped<IProductionOutputRepository, ProductionOutputRepository>();
            services.AddScoped<IProductionScrapRepository, ProductionScrapRepository>();
            services.AddScoped<IProductionDowntimeRepository, ProductionDowntimeRepository>();
            services.AddScoped<IProductionLaborLogRepository, ProductionLaborLogRepository>();
            services.AddScoped<IProductionQualityCheckRepository, ProductionQualityCheckRepository>();


            // Schedule
            services.AddScoped<ISchedulePlanRepository, SchedulePlanRepository>();
            services.AddScoped<IScheduleJobRepository, ScheduleJobRepository>();
            services.AddScoped<IScheduleOperationRepository, ScheduleOperationRepository>();
            services.AddScoped<IScheduleOperationDependencyRepository, ScheduleOperationDependencyRepository>();
            services.AddScoped<IScheduleOperationConstraintRepository, ScheduleOperationConstraintRepository>();
            services.AddScoped<IScheduleOperationResourceOptionRepository, ScheduleOperationResourceOptionRepository>();
            services.AddScoped<IScheduleResourceAssignmentRepository, ScheduleResourceAssignmentRepository>();
            services.AddScoped<IShiftRepository, ShiftRepository>();
            services.AddScoped<IResourceCalendarRepository, ResourceCalendarRepository>();
            services.AddScoped<IResourceCalendarExceptionRepository, ResourceCalendarExceptionRepository>();
            services.AddScoped<ICapacityReservationRepository, CapacityReservationRepository>();
            services.AddScoped<IResourceCapacitySnapshotRepository, ResourceCapacitySnapshotRepository>();
            services.AddScoped<IDispatchQueueRepository, DispatchQueueRepository>();
            services.AddScoped<IScheduleMaterialCheckRepository, ScheduleMaterialCheckRepository>();
            services.AddScoped<IScheduleExceptionRepository, ScheduleExceptionRepository>();
            services.AddScoped<IScheduleRevisionRepository, ScheduleRevisionRepository>();
            services.AddScoped<IScheduleRescheduleHistoryRepository, ScheduleRescheduleHistoryRepository>();
            services.AddScoped<IScheduleStatusHistoryRepository, ScheduleStatusHistoryRepository>();
            services.AddScoped<IScheduleAuditLogRepository, ScheduleAuditLogRepository>();


            // Shipment
            services.AddScoped<IShipmentRepository, ShipmentRepository>();
            services.AddScoped<ICarrierRepository, CarrierRepository>();
            services.AddScoped<IShipmentAddressRepository, ShipmentAddressRepository>();
            services.AddScoped<IDeliveryRunRepository, DeliveryRunRepository>();
            services.AddScoped<IDockAppointmentRepository, DockAppointmentRepository>();
            services.AddScoped<IReturnShipmentRepository, ReturnShipmentRepository>();
            services.AddScoped<IShipmentLookupRepository, ShipmentLookupRepository>();

            return services;
        }
    }
}
