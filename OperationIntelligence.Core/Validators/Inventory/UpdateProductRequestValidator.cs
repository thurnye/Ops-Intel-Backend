using FluentValidation;
namespace OperationIntelligence.Core;

public class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
{
    public UpdateProductRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.SKU)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.CategoryId)
            .NotEmpty();

        RuleFor(x => x.UnitOfMeasureId)
            .NotEmpty();

        RuleFor(x => x.CostPrice)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.SellingPrice)
            .GreaterThanOrEqualTo(0);
    }
}
