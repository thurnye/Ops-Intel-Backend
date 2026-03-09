using FluentValidation;
using OperationIntelligence.Core.Models.Production.Requests;

namespace OperationIntelligence.Core.Validators.Production;

public class CreateProductionOrderRequestValidator : AbstractValidator<CreateProductionOrderRequest>
{
    public CreateProductionOrderRequestValidator()
    {
        RuleFor(x => x.ProductionOrderNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.PlannedQuantity).GreaterThan(0);
        RuleFor(x => x.UnitOfMeasureId).NotEmpty();
        RuleFor(x => x.WarehouseId).NotEmpty();
        RuleFor(x => x.PlannedStartDate).NotEmpty();
        RuleFor(x => x.PlannedEndDate).GreaterThanOrEqualTo(x => x.PlannedStartDate);
        RuleFor(x => x.BatchNumber).MaximumLength(100);
        RuleFor(x => x.LotNumber).MaximumLength(100);
        RuleFor(x => x.Notes).MaximumLength(2000);
    }
}
