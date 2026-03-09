using FluentValidation;
using OperationIntelligence.Core.Models.Production.Requests;

namespace OperationIntelligence.Core.Validators.Production;

public class CreateBillOfMaterialItemRequestValidator : AbstractValidator<CreateBillOfMaterialItemRequest>
{
    public CreateBillOfMaterialItemRequestValidator()
    {
        RuleFor(x => x.BillOfMaterialId).NotEmpty();
        RuleFor(x => x.MaterialProductId).NotEmpty();
        RuleFor(x => x.UnitOfMeasureId).NotEmpty();
        RuleFor(x => x.QuantityRequired).GreaterThan(0);
        RuleFor(x => x.ScrapFactorPercent).InclusiveBetween(0, 100);
        RuleFor(x => x.YieldFactorPercent).GreaterThan(0).LessThanOrEqualTo(100);
        RuleFor(x => x.Sequence).GreaterThan(0);
        RuleFor(x => x.Notes).MaximumLength(1000);
    }
}
