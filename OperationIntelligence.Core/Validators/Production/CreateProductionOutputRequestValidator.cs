using FluentValidation;
using OperationIntelligence.Core.Models.Production.Requests;

namespace OperationIntelligence.Core.Validators.Production;

public class CreateProductionOutputRequestValidator : AbstractValidator<CreateProductionOutputRequest>
{
    public CreateProductionOutputRequestValidator()
    {
        RuleFor(x => x.ProductionOrderId).NotEmpty();
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.WarehouseId).NotEmpty();
        RuleFor(x => x.UnitOfMeasureId).NotEmpty();
        RuleFor(x => x.QuantityProduced).GreaterThan(0);
        RuleFor(x => x.BatchNumber).MaximumLength(100);
        RuleFor(x => x.LotNumber).MaximumLength(100);
        RuleFor(x => x.OutputDate).NotEmpty();
        RuleFor(x => x.Notes).MaximumLength(1000);
    }
}
