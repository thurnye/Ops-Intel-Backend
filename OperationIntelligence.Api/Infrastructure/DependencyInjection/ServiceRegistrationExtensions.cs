using OperationIntelligence.Core;

namespace OperationIntelligence.Api
{
    public static class ServiceRegistrationExtensions
    {
        public static IServiceCollection AddAppServices(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();

            services.AddScoped<BotDetectionService>();

            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<INormalizationService, NormalizationService>();
            services.AddScoped<IPasswordService, PasswordService>();
            services.AddScoped<ITokenService, TokenService>();

            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IInventoryStockService, InventoryStockService>();
            services.AddScoped<IStockMovementService, StockMovementService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IBrandService, BrandService>();
            services.AddScoped<IUnitOfMeasureService, UnitOfMeasureService>();
            services.AddScoped<IWarehouseService, WarehouseService>();
            services.AddScoped<ISupplierService, SupplierService>();
            services.AddScoped<IProductImageService, ProductImageService>();
            services.AddScoped<IProductSupplierService, ProductSupplierService>();
            

            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IOrderItemService, OrderItemService>();
            services.AddScoped<IOrderStatusHistoryService, OrderStatusHistoryService>();
            services.AddScoped<IOrderPaymentService, OrderPaymentService>();
            services.AddScoped<IOrderImageService, OrderImageService>();
            services.AddScoped<IOrderNoteService, OrderNoteService>();
            services.AddScoped<IOrderAddressService, OrderAddressService>();

            services.AddScoped<IWorkCenterService, WorkCenterService>();
            services.AddScoped<IMachineService, MachineService>();
            services.AddScoped<IBillOfMaterialService, BillOfMaterialService>();
            services.AddScoped<IBillOfMaterialItemService, BillOfMaterialItemService>();
            services.AddScoped<IRoutingService, RoutingService>();
            services.AddScoped<IRoutingStepService, RoutingStepService>();
            services.AddScoped<IProductionOrderService, ProductionOrderService>();
            services.AddScoped<IProductionExecutionService, ProductionExecutionService>();
            services.AddScoped<IProductionMaterialIssueService, ProductionMaterialIssueService>();
            services.AddScoped<IProductionMaterialConsumptionService, ProductionMaterialConsumptionService>();
            services.AddScoped<IProductionOutputService, ProductionOutputService>();
            services.AddScoped<IProductionScrapService, ProductionScrapService>();
            services.AddScoped<IProductionDowntimeService, ProductionDowntimeService>();
            services.AddScoped<IProductionLaborLogService, ProductionLaborLogService>();
            services.AddScoped<IProductionQualityCheckService, ProductionQualityCheckService>();

            services.AddScoped<ISchedulePlanService, SchedulePlanService>();
            services.AddScoped<IScheduleJobService, ScheduleJobService>();
            services.AddScoped<IScheduleOperationService, ScheduleOperationService>();
            services.AddScoped<IShiftService, ShiftService>();
            services.AddScoped<IResourceCalendarService, ResourceCalendarService>();
            services.AddScoped<ICapacityService, CapacityService>();
            services.AddScoped<IDispatchService, DispatchService>();
            services.AddScoped<IScheduleMaterialService, ScheduleMaterialService>();
            services.AddScoped<IScheduleExceptionService, ScheduleExceptionService>();
            services.AddScoped<IScheduleRevisionService, ScheduleRevisionService>();
            services.AddScoped<IScheduleAuditService, ScheduleAuditService>();

            // Shipment
            services.AddScoped<IShipmentService, ShipmentService>();
            services.AddScoped<ICarrierService, ShipmentCarrierService>();
            services.AddScoped<IShipmentAddressService, ShipmentAddressService>();
            services.AddScoped<IDeliveryRunService, DeliveryRunService>();
            services.AddScoped<IDockAppointmentService, DockAppointmentService>();
            services.AddScoped<IReturnShipmentService, ReturnShipmentService>();
            services.AddScoped<IShipmentLookupService, ShipmentLookupService>();

            services.AddScoped<CacheInvalidationService>();

            return services;
        }
    }
}
