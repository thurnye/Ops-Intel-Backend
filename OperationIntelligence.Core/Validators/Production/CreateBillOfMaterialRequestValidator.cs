using FluentValidation;
using OperationIntelligence.Core.Models.Production.Requests;

namespace OperationIntelligence.Core.Validators.Production;

public class CreateBillOfMaterialRequestValidator : AbstractValidator<CreateBillOfMaterialRequest>
{
    public CreateBillOfMaterialRequestValidator()
    {
        RuleFor(x => x.BomCode).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.UnitOfMeasureId).NotEmpty();
        RuleFor(x => x.BaseQuantity).GreaterThan(0);
        RuleFor(x => x.Version).GreaterThan(0);
        RuleFor(x => x.EffectiveTo)
            .GreaterThanOrEqualTo(x => x.EffectiveFrom)
            .When(x => x.EffectiveTo.HasValue);
        RuleFor(x => x.Notes).MaximumLength(1000);
    }
}
