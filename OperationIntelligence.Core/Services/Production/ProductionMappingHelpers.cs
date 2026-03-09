using OperationIntelligence.DB;

namespace OperationIntelligence.Core;

internal static class ProductionMappingHelpers
{
    public static WorkCenterResponse ToResponse(this WorkCenter entity) => new()
    {
        Id = entity.Id,
        Code = entity.Code,
        Name = entity.Name,
        Description = entity.Description,
        WarehouseId = entity.WarehouseId,
        WarehouseName = entity.Warehouse?.Name,
        CapacityPerDay = entity.CapacityPerDay,
        AvailableOperators = entity.AvailableOperators,
        IsActive = entity.IsActive,
        CreatedAtUtc = entity.CreatedAtUtc,
        CreatedBy = entity.CreatedBy,
        UpdatedAtUtc = entity.UpdatedAtUtc,
        UpdatedBy = entity.UpdatedBy
    };

    public static MachineResponse ToResponse(this Machine entity) => new()
    {
        Id = entity.Id,
        MachineCode = entity.MachineCode,
        Name = entity.Name,
        WorkCenterId = entity.WorkCenterId,
        WorkCenterName = entity.WorkCenter?.Name,
        Model = entity.Model,
        Manufacturer = entity.Manufacturer,
        SerialNumber = entity.SerialNumber,
        HourlyRunningCost = entity.HourlyRunningCost,
        Status = entity.Status,
        LastMaintenanceDate = entity.LastMaintenanceDate,
        NextMaintenanceDate = entity.NextMaintenanceDate,
        IsActive = entity.IsActive,
        CreatedAtUtc = entity.CreatedAtUtc,
        CreatedBy = entity.CreatedBy,
        UpdatedAtUtc = entity.UpdatedAtUtc,
        UpdatedBy = entity.UpdatedBy
    };

    public static BillOfMaterialItemResponse ToResponse(this BillOfMaterialItem entity) => new()
    {
        Id = entity.Id,
        BillOfMaterialId = entity.BillOfMaterialId,
        MaterialProductId = entity.MaterialProductId,
        MaterialProductName = entity.MaterialProduct?.Name,
        MaterialProductSku = entity.MaterialProduct?.SKU,
        QuantityRequired = entity.QuantityRequired,
        UnitOfMeasureId = entity.UnitOfMeasureId,
        UnitOfMeasureName = entity.UnitOfMeasure?.Name,
        ScrapFactorPercent = entity.ScrapFactorPercent,
        YieldFactorPercent = entity.YieldFactorPercent,
        IsOptional = entity.IsOptional,
        IsBackflush = entity.IsBackflush,
        Sequence = entity.Sequence,
        Notes = entity.Notes
    };

    public static BillOfMaterialResponse ToResponse(this BillOfMaterial entity) => new()
    {
        Id = entity.Id,
        BomCode = entity.BomCode,
        Name = entity.Name,
        ProductId = entity.ProductId,
        ProductName = entity.Product?.Name,
        ProductSku = entity.Product?.SKU,
        BaseQuantity = entity.BaseQuantity,
        UnitOfMeasureId = entity.UnitOfMeasureId,
        UnitOfMeasureName = entity.UnitOfMeasure?.Name,
        Version = entity.Version,
        IsActive = entity.IsActive,
        IsDefault = entity.IsDefault,
        EffectiveFrom = entity.EffectiveFrom,
        EffectiveTo = entity.EffectiveTo,
        Notes = entity.Notes,
        Items = entity.Items.Select(ToResponse).OrderBy(x => x.Sequence).ToList(),
        CreatedAtUtc = entity.CreatedAtUtc,
        CreatedBy = entity.CreatedBy,
        UpdatedAtUtc = entity.UpdatedAtUtc,
        UpdatedBy = entity.UpdatedBy
    };

    public static RoutingStepResponse ToResponse(this RoutingStep entity) => new()
    {
        Id = entity.Id,
        RoutingId = entity.RoutingId,
        Sequence = entity.Sequence,
        OperationCode = entity.OperationCode,
        OperationName = entity.OperationName,
        WorkCenterId = entity.WorkCenterId,
        WorkCenterName = entity.WorkCenter?.Name,
        SetupTimeMinutes = entity.SetupTimeMinutes,
        RunTimeMinutesPerUnit = entity.RunTimeMinutesPerUnit,
        QueueTimeMinutes = entity.QueueTimeMinutes,
        WaitTimeMinutes = entity.WaitTimeMinutes,
        MoveTimeMinutes = entity.MoveTimeMinutes,
        RequiredOperators = entity.RequiredOperators,
        IsOutsourced = entity.IsOutsourced,
        IsQualityCheckpointRequired = entity.IsQualityCheckpointRequired,
        Instructions = entity.Instructions,
        Notes = entity.Notes
    };

    public static RoutingResponse ToResponse(this Routing entity) => new()
    {
        Id = entity.Id,
        RoutingCode = entity.RoutingCode,
        Name = entity.Name,
        ProductId = entity.ProductId,
        ProductName = entity.Product?.Name,
        ProductSku = entity.Product?.SKU,
        Version = entity.Version,
        IsActive = entity.IsActive,
        IsDefault = entity.IsDefault,
        EffectiveFrom = entity.EffectiveFrom,
        EffectiveTo = entity.EffectiveTo,
        Notes = entity.Notes,
        Steps = entity.Steps.Select(ToResponse).OrderBy(x => x.Sequence).ToList(),
        CreatedAtUtc = entity.CreatedAtUtc,
        CreatedBy = entity.CreatedBy,
        UpdatedAtUtc = entity.UpdatedAtUtc,
        UpdatedBy = entity.UpdatedBy
    };

    public static ProductionMaterialConsumptionResponse ToResponse(this ProductionMaterialConsumption entity) => new()
    {
        Id = entity.Id,
        ProductionMaterialIssueId = entity.ProductionMaterialIssueId,
        ProductionExecutionId = entity.ProductionExecutionId,
        ConsumedQuantity = entity.ConsumedQuantity,
        ConsumptionDate = entity.ConsumptionDate,
        Notes = entity.Notes,
        CreatedAtUtc = entity.CreatedAtUtc,
        CreatedBy = entity.CreatedBy
    };

    public static ProductionMaterialIssueResponse ToResponse(this ProductionMaterialIssue entity) => new()
    {
        Id = entity.Id,
        ProductionOrderId = entity.ProductionOrderId,
        MaterialProductId = entity.MaterialProductId,
        MaterialProductName = entity.MaterialProduct?.Name,
        MaterialProductSku = entity.MaterialProduct?.SKU,
        WarehouseId = entity.WarehouseId,
        WarehouseName = entity.Warehouse?.Name,
        PlannedQuantity = entity.PlannedQuantity,
        IssuedQuantity = entity.IssuedQuantity,
        ReturnedQuantity = entity.ReturnedQuantity,
        UnitOfMeasureId = entity.UnitOfMeasureId,
        UnitOfMeasureName = entity.UnitOfMeasure?.Name,
        BatchNumber = entity.BatchNumber,
        LotNumber = entity.LotNumber,
        IssueDate = entity.IssueDate,
        StockMovementId = entity.StockMovementId,
        Notes = entity.Notes,
        Consumptions = entity.Consumptions.Select(ToResponse).ToList(),
        CreatedAtUtc = entity.CreatedAtUtc,
        CreatedBy = entity.CreatedBy,
        UpdatedAtUtc = entity.UpdatedAtUtc,
        UpdatedBy = entity.UpdatedBy
    };

    public static ProductionOutputResponse ToResponse(this ProductionOutput entity) => new()
    {
        Id = entity.Id,
        ProductionOrderId = entity.ProductionOrderId,
        ProductId = entity.ProductId,
        ProductName = entity.Product?.Name,
        ProductSku = entity.Product?.SKU,
        WarehouseId = entity.WarehouseId,
        WarehouseName = entity.Warehouse?.Name,
        QuantityProduced = entity.QuantityProduced,
        UnitOfMeasureId = entity.UnitOfMeasureId,
        UnitOfMeasureName = entity.UnitOfMeasure?.Name,
        BatchNumber = entity.BatchNumber,
        LotNumber = entity.LotNumber,
        OutputDate = entity.OutputDate,
        StockMovementId = entity.StockMovementId,
        IsFinalOutput = entity.IsFinalOutput,
        Notes = entity.Notes,
        CreatedAtUtc = entity.CreatedAtUtc,
        CreatedBy = entity.CreatedBy,
        UpdatedAtUtc = entity.UpdatedAtUtc,
        UpdatedBy = entity.UpdatedBy
    };

    public static ProductionScrapResponse ToResponse(this ProductionScrap entity) => new()
    {
        Id = entity.Id,
        ProductionOrderId = entity.ProductionOrderId,
        ProductionExecutionId = entity.ProductionExecutionId,
        ProductId = entity.ProductId,
        ProductName = entity.Product?.Name,
        ProductSku = entity.Product?.SKU,
        ScrapQuantity = entity.ScrapQuantity,
        UnitOfMeasureId = entity.UnitOfMeasureId,
        UnitOfMeasureName = entity.UnitOfMeasure?.Name,
        Reason = entity.Reason,
        ReasonDescription = entity.ReasonDescription,
        ScrapDate = entity.ScrapDate,
        IsReworkable = entity.IsReworkable,
        Notes = entity.Notes,
        CreatedAtUtc = entity.CreatedAtUtc,
        CreatedBy = entity.CreatedBy,
        UpdatedAtUtc = entity.UpdatedAtUtc,
        UpdatedBy = entity.UpdatedBy
    };

    public static ProductionDowntimeResponse ToResponse(this ProductionDowntime entity) => new()
    {
        Id = entity.Id,
        ProductionExecutionId = entity.ProductionExecutionId,
        Reason = entity.Reason,
        ReasonDescription = entity.ReasonDescription,
        StartTime = entity.StartTime,
        EndTime = entity.EndTime,
        DurationMinutes = entity.DurationMinutes,
        IsPlanned = entity.IsPlanned,
        Notes = entity.Notes,
        CreatedAtUtc = entity.CreatedAtUtc,
        CreatedBy = entity.CreatedBy,
        UpdatedAtUtc = entity.UpdatedAtUtc,
        UpdatedBy = entity.UpdatedBy
    };

    public static ProductionLaborLogResponse ToResponse(this ProductionLaborLog entity) => new()
    {
        Id = entity.Id,
        ProductionExecutionId = entity.ProductionExecutionId,
        UserId = entity.UserId,
        UserName = entity.User == null ? null : $@"{entity.User.FirstName} {entity.User.LastName}".Trim(),
        UserEmail = entity.User?.Email,
        HoursWorked = entity.HoursWorked,
        HourlyRate = entity.HourlyRate,
        WorkDate = entity.WorkDate,
        Notes = entity.Notes,
        CreatedAtUtc = entity.CreatedAtUtc,
        CreatedBy = entity.CreatedBy,
        UpdatedAtUtc = entity.UpdatedAtUtc,
        UpdatedBy = entity.UpdatedBy
    };

    public static ProductionQualityCheckResponse ToResponse(this ProductionQualityCheck entity) => new()
    {
        Id = entity.Id,
        ProductionOrderId = entity.ProductionOrderId,
        ProductionExecutionId = entity.ProductionExecutionId,
        CheckType = entity.CheckType,
        Status = entity.Status,
        CheckDate = entity.CheckDate,
        CheckedByUserId = entity.CheckedByUserId,
        CheckedByUserName = entity.CheckedByUser == null ? null : $@"{entity.CheckedByUser.FirstName} {entity.CheckedByUser.LastName}".Trim(),
        CheckedByUserEmail = entity.CheckedByUser?.Email,
        ReferenceStandard = entity.ReferenceStandard,
        Findings = entity.Findings,
        CorrectiveAction = entity.CorrectiveAction,
        RequiresRework = entity.RequiresRework,
        Notes = entity.Notes,
        CreatedAtUtc = entity.CreatedAtUtc,
        CreatedBy = entity.CreatedBy,
        UpdatedAtUtc = entity.UpdatedAtUtc,
        UpdatedBy = entity.UpdatedBy
    };

    public static ProductionExecutionResponse ToResponse(this ProductionExecution entity) => new()
    {
        Id = entity.Id,
        ProductionOrderId = entity.ProductionOrderId,
        ProductionOrderNumber = entity.ProductionOrder?.ProductionOrderNumber,
        RoutingStepId = entity.RoutingStepId,
        RoutingStepSequence = entity.RoutingStep?.Sequence,
        OperationCode = entity.RoutingStep?.OperationCode,
        OperationName = entity.RoutingStep?.OperationName,
        WorkCenterId = entity.WorkCenterId,
        WorkCenterName = entity.WorkCenter?.Name,
        MachineId = entity.MachineId,
        MachineName = entity.Machine?.Name,
        MachineCode = entity.Machine?.MachineCode,
        PlannedQuantity = entity.PlannedQuantity,
        CompletedQuantity = entity.CompletedQuantity,
        ScrapQuantity = entity.ScrapQuantity,
        PlannedStartDate = entity.PlannedStartDate,
        PlannedEndDate = entity.PlannedEndDate,
        ActualStartDate = entity.ActualStartDate,
        ActualEndDate = entity.ActualEndDate,
        ActualSetupTimeMinutes = entity.ActualSetupTimeMinutes,
        ActualRunTimeMinutes = entity.ActualRunTimeMinutes,
        ActualDowntimeMinutes = entity.ActualDowntimeMinutes,
        Status = entity.Status,
        Remarks = entity.Remarks,
        MaterialConsumptions = entity.MaterialConsumptions.Select(ToResponse).ToList(),
        LaborLogs = entity.LaborLogs.Select(ToResponse).ToList(),
        Downtimes = entity.Downtimes.Select(ToResponse).ToList(),
        Scraps = entity.Scraps.Select(ToResponse).ToList(),
        QualityChecks = entity.QualityChecks.Select(ToResponse).ToList(),
        CreatedAtUtc = entity.CreatedAtUtc,
        CreatedBy = entity.CreatedBy,
        UpdatedAtUtc = entity.UpdatedAtUtc,
        UpdatedBy = entity.UpdatedBy
    };

    public static ProductionOrderResponse ToResponse(this ProductionOrder entity) => new()
    {
        Id = entity.Id,
        ProductionOrderNumber = entity.ProductionOrderNumber,
        ProductId = entity.ProductId,
        ProductName = entity.Product?.Name,
        ProductSku = entity.Product?.SKU,
        PlannedQuantity = entity.PlannedQuantity,
        ProducedQuantity = entity.ProducedQuantity,
        ScrapQuantity = entity.ScrapQuantity,
        RemainingQuantity = entity.RemainingQuantity,
        UnitOfMeasureId = entity.UnitOfMeasureId,
        UnitOfMeasureName = entity.UnitOfMeasure?.Name,
        BillOfMaterialId = entity.BillOfMaterialId,
        BillOfMaterialCode = entity.BillOfMaterial?.BomCode,
        BillOfMaterialName = entity.BillOfMaterial?.Name,
        RoutingId = entity.RoutingId,
        RoutingCode = entity.Routing?.RoutingCode,
        RoutingName = entity.Routing?.Name,
        WarehouseId = entity.WarehouseId,
        WarehouseName = entity.Warehouse?.Name,
        PlannedStartDate = entity.PlannedStartDate,
        PlannedEndDate = entity.PlannedEndDate,
        ActualStartDate = entity.ActualStartDate,
        ActualEndDate = entity.ActualEndDate,
        Status = entity.Status,
        Priority = entity.Priority,
        SourceType = entity.SourceType,
        SourceReferenceId = entity.SourceReferenceId,
        BatchNumber = entity.BatchNumber,
        LotNumber = entity.LotNumber,
        Notes = entity.Notes,
        EstimatedMaterialCost = entity.EstimatedMaterialCost,
        EstimatedLaborCost = entity.EstimatedLaborCost,
        EstimatedOverheadCost = entity.EstimatedOverheadCost,
        ActualMaterialCost = entity.ActualMaterialCost,
        ActualLaborCost = entity.ActualLaborCost,
        ActualOverheadCost = entity.ActualOverheadCost,
        IsReleased = entity.IsReleased,
        IsClosed = entity.IsClosed,
        Executions = entity.Executions.Select(ToResponse).ToList(),
        MaterialIssues = entity.MaterialIssues.Select(ToResponse).ToList(),
        Outputs = entity.Outputs.Select(ToResponse).ToList(),
        Scraps = entity.Scraps.Select(ToResponse).ToList(),
        QualityChecks = entity.QualityChecks.Select(ToResponse).ToList(),
        CreatedAtUtc = entity.CreatedAtUtc,
        CreatedBy = entity.CreatedBy,
        UpdatedAtUtc = entity.UpdatedAtUtc,
        UpdatedBy = entity.UpdatedBy
    };
}
